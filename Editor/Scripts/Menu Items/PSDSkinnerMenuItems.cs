using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.U2D.Animation;
using UnityEditor.U2D.PSD;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.U2D;

namespace HHG.Common.Editor
{
    public static class PSDSkinnerMenuItems
    {
        private const string menuRoot = "| Half Human Games |/Tools/PSD/";

        private static Assembly animEditorAssembly;
        private static Type spriteMeshDataType;
        private static Type spriteMeshDataControllerType;
        private static Type outlineGeneratorType;
        private static Type triangulatorType;
        private static FieldInfo spriteMeshDataControllerField;
        private static MethodInfo setFrameMethod;
        private static MethodInfo outlineFromAlphaMethod;
        private static MethodInfo triangulateMethod;
        private static PropertyInfo verticesProperty;
        private static PropertyInfo indicesProperty;
        private static PropertyInfo edgesProperty;
        private static FieldInfo edgeXField;
        private static FieldInfo edgeYField;
        private static bool reflectionInitialized;
        private static bool reflectionValid;
        private static IPSDSkinner psdSkinner;

        [MenuItem(menuRoot + "Skin Sprites")]
        private static void SkinSpritesMenuItem()
        {
            List<Type> implementations = FindIPSDSkinnerImplementations();

            if (implementations.Count == 0)
            {
                Debug.LogError("[SkinningSetup] No IPSDSkinner implementation found in any loaded assembly.");
                return;
            }

            if (implementations.Count == 1)
            {
                psdSkinner = (IPSDSkinner)Activator.CreateInstance(implementations[0]);
                SetupSkinningOnSelection();
                return;
            }

            implementations.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            string[] names = implementations.ConvertAll(t => t.Name).ToArray();
            Rect parentRect = EditorWindow.focusedWindow?.position ?? new Rect(0, 0, 800, 600);

            SkinnerPickerWindow.Show(names, parentRect, (Action<int>)(chosenIndex =>
            {
                psdSkinner = (IPSDSkinner)Activator.CreateInstance(implementations[chosenIndex]);
                SetupSkinningOnSelection();
            }));
        }

        [MenuItem(menuRoot + "Skin Sprites", validate = true)]
        private static bool SkinSpritesValidate() => IsValidSelection();

        private static List<Type> FindIPSDSkinnerImplementations()
        {
            List<Type> results = new List<Type>();
            Type iface = typeof(IPSDSkinner);
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type t in asm.GetTypes())
                    {
                        if (t.IsClass && !t.IsAbstract && iface.IsAssignableFrom(t))
                            results.Add(t);
                    }
                }
                catch { } // Skip inaccessible assemblies
            }
            return results;
        }

        private static bool IsValidSelection()
        {
            foreach (string path in GetSelectedPSBPaths())
            {
                if (AssetImporter.GetAtPath(path) is PSDImporter)
                {
                    return true;
                }
            }
            return false;
        }

        private static string[] GetSelectedPSBPaths()
        {
            return Array.ConvertAll(Selection.assetGUIDs, AssetDatabase.GUIDToAssetPath);
        }

        private static void SetupSkinningOnSelection()
        {
            foreach (string path in GetSelectedPSBPaths())
            {
                if (AssetImporter.GetAtPath(path) is PSDImporter)
                {
                    SetupSkinning(path);
                }
            }
        }

        private static void SetupSkinning(string path)
        {
            SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
            factory.Init();
            ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(AssetImporter.GetAtPath(path));
            dataProvider.InitSpriteEditorDataProvider();

            ISpriteBoneDataProvider boneProvider = dataProvider.GetDataProvider<ISpriteBoneDataProvider>();
            ISpriteMeshDataProvider meshProvider = dataProvider.GetDataProvider<ISpriteMeshDataProvider>();
            ITextureDataProvider textureProvider = dataProvider.GetDataProvider<ITextureDataProvider>();
            IMainSkeletonDataProvider skeletonProvider = dataProvider.GetDataProvider<IMainSkeletonDataProvider>();
            ICharacterDataProvider characterProvider = dataProvider.GetDataProvider<ICharacterDataProvider>();

            SpriteBone[] skeletonBones = skeletonProvider.GetMainSkeletonData().bones ?? Array.Empty<SpriteBone>();
            int pelvisBoneIndex = Array.FindIndex(skeletonBones, b => b.name == psdSkinner.PelvisBoneName);

            SpriteRect[] spriteRects = dataProvider.GetSpriteRects();

            CharacterData characterData = characterProvider.GetCharacterData();
            characterData.bones = skeletonBones;
            characterData.parts ??= Array.Empty<CharacterPart>();
            List<CharacterPart> characterParts = new List<CharacterPart>(characterData.parts);

            int processed = 0, skippedWeights = 0, failed = 0;
            foreach (SpriteRect spriteRect in spriteRects)
            {
                GUID guid = spriteRect.spriteID;

                if (!TryAutoGenerateGeometry(textureProvider, spriteRect, out Vector2[] vertices, out int[] indices, out Vector2Int[] edges))
                {
                    failed++;
                    continue;
                }

                string boneName = psdSkinner.MapSpriteToBone(spriteRect.name);
                int boneIndex = string.IsNullOrEmpty(boneName) ? -1 : Array.FindIndex(skeletonBones, b => b.name == boneName);

                if (!string.IsNullOrEmpty(boneName) && boneIndex < 0)
                {
                    Debug.LogWarning($"[SkinningSetup] Bone '{boneName}' not found in skeleton for sprite '{spriteRect.name}'.");
                }

                boneProvider.SetBones(guid, new List<SpriteBone>(skeletonBones));

                bool isTorso = psdSkinner.IsTorsoSprite(boneName) && pelvisBoneIndex >= 0;
                CharacterPart? torsoPart = isTorso ? FindCharacterPart(characterParts, guid) : null;
                float boneWorldY = isTorso ? GetBoneWorldY(skeletonBones, boneIndex) : 0f;
                float torsoThreshold = isTorso && torsoPart.HasValue ? GetBoneSpriteLocalY(torsoPart.Value, boneWorldY) : 0f;

                Vertex2DMetaData[] metaVertices = new Vertex2DMetaData[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    metaVertices[i].position = vertices[i];
                    if (isTorso)
                    {
                        float torsoWeight = Mathf.InverseLerp(
                            torsoThreshold - psdSkinner.TorsoBlendHalfWidth,
                            torsoThreshold + psdSkinner.TorsoBlendHalfWidth,
                            vertices[i].y);

                        metaVertices[i].boneWeight = new BoneWeight
                        {
                            boneIndex0 = 0,
                            weight0 = torsoWeight,
                            boneIndex1 = 1,
                            weight1 = 1f - torsoWeight
                        };
                    }
                    else if (boneIndex >= 0)
                    {
                        metaVertices[i].boneWeight = new BoneWeight { boneIndex0 = 0, weight0 = 1f };
                    }
                }

                meshProvider.SetVertices(guid, metaVertices);
                meshProvider.SetIndices(guid, indices);
                meshProvider.SetEdges(guid, edges);

                if (boneIndex >= 0)
                {
                    string spriteIdStr = guid.ToString();
                    int existingIndex = characterParts.FindIndex(p => p.spriteId == spriteIdStr);
                    CharacterPart part = existingIndex >= 0 ? characterParts[existingIndex] : new CharacterPart();
                    part.spriteId = spriteIdStr;
                    part.bones = isTorso ? new int[] { boneIndex, pelvisBoneIndex } : new int[] { boneIndex };
                    if (existingIndex >= 0) characterParts[existingIndex] = part;
                    else characterParts.Add(part);
                }

                if (boneIndex < 0) skippedWeights++;

                processed++;
            }

            characterData.parts = characterParts.ToArray();
            characterProvider.SetCharacterData(characterData);
            dataProvider.Apply();

            PSDImporter importer = AssetImporter.GetAtPath(path) as PSDImporter;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            Debug.Log($"[SkinningSetup] '{path}': {processed} processed, {skippedWeights} skipped weights, {failed} failed.");
        }

        private static CharacterPart? FindCharacterPart(List<CharacterPart> characterParts, GUID guid)
        {
            string spriteId = guid.ToString();
            int index = characterParts.FindIndex(p => p.spriteId == spriteId);
            return index >= 0 ? characterParts[index] : null;
        }

        private static float GetBoneWorldY(SpriteBone[] bones, int index)
        {
            List<int> chain = new List<int>();

            int current = index;
            while (current >= 0)
            {
                chain.Add(current);
                current = bones[current].parentId;
            }

            chain.Reverse();

            Vector2 worldPos = Vector2.zero;
            float worldAngleDeg = 0f;

            foreach (int boneIdx in chain)
            {
                SpriteBone bone = bones[boneIdx];
                float rad = worldAngleDeg * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);
                float lx = bone.position.x;
                float ly = bone.position.y;
                worldPos.x += cos * lx - sin * ly;
                worldPos.y += sin * lx + cos * ly;
                worldAngleDeg += bone.rotation.eulerAngles.z;
            }

            return worldPos.y;
        }

        private static float GetBoneSpriteLocalY(CharacterPart part, float boneCanvasY)
        {
            return boneCanvasY - part.spritePosition.yMin;
        }

        private static bool TryAutoGenerateGeometry(
            ITextureDataProvider textureProvider,
            SpriteRect spriteRect,
            out Vector2[] vertices,
            out int[] indices,
            out Vector2Int[] edges)
        {
            vertices = null;
            indices = null;
            edges = null;

            if (!EnsureReflection()) return false;

            try
            {
                object spriteMeshData = Activator.CreateInstance(spriteMeshDataType);
                setFrameMethod.Invoke(spriteMeshData, new object[] { spriteRect.rect });

                object spriteMeshDataController = Activator.CreateInstance(spriteMeshDataControllerType);
                spriteMeshDataControllerField.SetValue(spriteMeshDataController, spriteMeshData);

                object outlineGenerator = Activator.CreateInstance(outlineGeneratorType);
                outlineFromAlphaMethod.Invoke(spriteMeshDataController, new object[] { outlineGenerator, textureProvider, psdSkinner.OutlineDetail, (byte)100 });

                object triangulator = Activator.CreateInstance(triangulatorType);
                triangulateMethod.Invoke(spriteMeshDataController, new object[] { triangulator });

                vertices = (Vector2[])verticesProperty.GetValue(spriteMeshData);
                indices = (int[])indicesProperty.GetValue(spriteMeshData);
                edges = ConvertEdgesToVector2Int(edgesProperty.GetValue(spriteMeshData));

                return vertices != null && indices != null && edges != null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SkinningSetup] Auto-geometry exception for '{spriteRect.name}': {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }
        }

        private static Vector2Int[] ConvertEdgesToVector2Int(object rawEdges)
        {
            Array arr = (Array)rawEdges;
            Vector2Int[] result = new Vector2Int[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                object elem = arr.GetValue(i);
                result[i] = new Vector2Int((int)edgeXField.GetValue(elem), (int)edgeYField.GetValue(elem));
            }
            return result;
        }

        private static bool EnsureReflection()
        {
            if (reflectionInitialized) return reflectionValid;

            reflectionInitialized = true;

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name == "Unity.2D.Animation.Editor")
                {
                    animEditorAssembly = asm;
                    break;
                }
            }

            if (animEditorAssembly == null)
            {
                Debug.LogError("[SkinningSetup] Could not find assembly 'Unity.2D.Animation.Editor'.");
                return false;
            }

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            spriteMeshDataType = animEditorAssembly.GetType("UnityEditor.U2D.Animation.SpriteMeshData");
            spriteMeshDataControllerType = animEditorAssembly.GetType("UnityEditor.U2D.Animation.SpriteMeshDataController");
            outlineGeneratorType = animEditorAssembly.GetType("UnityEditor.U2D.Animation.OutlineGenerator");
            triangulatorType = animEditorAssembly.GetType("UnityEditor.U2D.Animation.Triangulator");

            if (spriteMeshDataType == null || 
                spriteMeshDataControllerType == null ||
                outlineGeneratorType == null || 
                triangulatorType == null)
            {
                Debug.LogError("[SkinningSetup] Could not resolve internal skinning types — com.unity.2d.animation API may have changed.");
                return false;
            }

            spriteMeshDataControllerField = spriteMeshDataControllerType.GetField("spriteMeshData", flags);
            setFrameMethod = spriteMeshDataType.GetMethod("SetFrame", flags);
            outlineFromAlphaMethod = spriteMeshDataControllerType.GetMethod("OutlineFromAlpha", flags);
            verticesProperty = spriteMeshDataType.GetProperty("vertices", flags);
            indicesProperty = spriteMeshDataType.GetProperty("indices", flags);
            edgesProperty = spriteMeshDataType.GetProperty("edges", flags);

            foreach (MethodInfo m in spriteMeshDataControllerType.GetMethods(flags))
            {
                if (m.Name != "Triangulate") continue;

                ParameterInfo[] ps = m.GetParameters();
                if (ps.Length == 1 && ps[0].ParameterType.IsAssignableFrom(triangulatorType))
                {
                    triangulateMethod = m;
                    break;
                }
            }

            Type edgeElementType = edgesProperty?.PropertyType.GetElementType();
            edgeXField = edgeElementType?.GetField("x");
            edgeYField = edgeElementType?.GetField("y");

            if (spriteMeshDataControllerField == null || 
                setFrameMethod == null ||
                outlineFromAlphaMethod == null || 
                triangulateMethod == null ||
                verticesProperty == null || 
                indicesProperty == null || 
                edgesProperty == null ||
                edgeXField == null || 
                edgeYField == null)
            {
                Debug.LogError("[SkinningSetup] Could not bind internal skinning members — com.unity.2d.animation API may have changed.");
                return false;
            }

            reflectionValid = true;
            return true;
        }

        private class SkinnerPickerWindow : EditorWindow
        {
            private string[] names;
            private Action<int> onSelected;

            public static void Show(string[] names, Rect parentRect, Action<int> onSelected)
            {
                SkinnerPickerWindow window = CreateInstance<SkinnerPickerWindow>();
                window.names = names;
                window.onSelected = onSelected;
                window.titleContent = new GUIContent("Select Skinner");

                float rowHeight = EditorGUIUtility.singleLineHeight + 4f;
                float height = names.Length * rowHeight + 8f;
                float x = parentRect.x + (parentRect.width - 240f) * 0.5f;
                float y = parentRect.y + (parentRect.height - height) * 0.5f;

                window.position = new Rect(x, y, 240f, height);
                window.ShowPopup();
            }

            private void OnGUI()
            {
                for (int i = 0; i < names.Length; i++)
                {
                    if (GUILayout.Button(names[i]))
                    {
                        int captured = i;
                        Close();
                        onSelected?.Invoke(captured);
                        return;
                    }
                }

                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
                {
                    Close();
                }
            }

            private void OnLostFocus()
            {
                Close();
            }
        }
    }
}