using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    public class FontScalerTool : EditorWindow
    {
        [SerializeField] private Font sourceFont;
        [SerializeField] private Vector2 scale = new Vector2(0.8f, 1f);
        [SerializeField] private bool autoSuffix = true;
        [SerializeField] private string customSuffix = " Condensed";
        [SerializeField] private bool createTmpAsset = true;
        [SerializeField] private int extraSpacing = 0;
        [SerializeField] private bool preventOverhang = false;

        [MenuItem("| Half Human Games |/Tools/Font Scaler")]
        private static void Open() => GetWindow<FontScalerTool>("Font Scaler");

        [MenuItem("Assets/Tools/Scale Font...", false, 2000)]
        private static void OpenFromSelection()
        {
            FontScalerTool window = GetWindow<FontScalerTool>("Font Scaler");
            window.sourceFont = Selection.activeObject as Font;
        }

        [MenuItem("Assets/Tools/Scale Font...", true)]
        private static bool ValidateSelection() => Selection.activeObject is Font;

        private string Suffix => autoSuffix
            ? $" X{Mathf.RoundToInt(scale.x * 100)} Y{Mathf.RoundToInt(scale.y * 100)}"
            : customSuffix;

        private void OnGUI()
        {
            EditorGUILayout.Space();
            sourceFont = (Font)EditorGUILayout.ObjectField("Base Font", sourceFont, typeof(Font), false);

            scale = EditorGUILayout.Vector2Field(new GUIContent("Scale (X, Y)", "1 = unchanged. 0.8 X = 80% width."), scale);
            scale.x = Mathf.Clamp(scale.x, 0.05f, 10f);
            scale.y = Mathf.Clamp(scale.y, 0.05f, 10f);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("80% width"))
            {
                scale = new Vector2(0.8f, 1f);
            }
            if (GUILayout.Button("80% height"))
            {
                scale = new Vector2(1f, 0.8f);
            }
            if (GUILayout.Button("Reset"))
            {
                scale = Vector2.one;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            extraSpacing = EditorGUILayout.IntField(new GUIContent("Extra Letter Spacing",
                "Font units added to every letter's width (most fonts are 1000 or 2048 units per em). Negative tightens."), extraSpacing);
            preventOverhang = EditorGUILayout.Toggle(new GUIContent("Prevent Overhang",
                "Widens letters whose shape sticks out past their own width (e.g. the bar of a T), so neighbouring letters can't overlap."), preventOverhang);

            EditorGUILayout.Space();
            autoSuffix = EditorGUILayout.Toggle(new GUIContent("Auto Name Suffix", "Appends e.g. ' X80 Y100' to the font name and file name."), autoSuffix);
            using (new EditorGUI.DisabledScope(autoSuffix))
            {
                customSuffix = EditorGUILayout.TextField("Name Suffix", autoSuffix ? Suffix : customSuffix);
            }

            createTmpAsset = EditorGUILayout.Toggle("Create TMP Font Asset", createTmpAsset);

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Works with TrueType fonts (.ttf). Outlines, advance widths, kerning (kern + GPOS) and line metrics are all scaled, " +
                "so spacing and word wrap stay correct. Hinting is removed. Re-running with the same suffix overwrites the previous output.",
                MessageType.Info);

            using (new EditorGUI.DisabledScope(sourceFont == null))
            {
                if (GUILayout.Button("Generate Scaled Font", GUILayout.Height(30)))
                {
                    Generate();
                }
            }
        }

        private void Generate()
        {
            string srcPath = AssetDatabase.GetAssetPath(sourceFont);
            if (string.IsNullOrEmpty(srcPath) || !File.Exists(srcPath))
            {
                EditorUtility.DisplayDialog("Font Scaler", "Couldn't find the font file for this asset.", "OK");
                return;
            }

            string suffix = Suffix;
            List<string> warnings = new List<string>();
            byte[] scaled;
            try
            {
                scaled = FontUtil.Scale(File.ReadAllBytes(srcPath), scale.x, scale.y, suffix, warnings, extraSpacing, preventOverhang);
            }
            catch (System.Exception exception)
            {
                EditorUtility.DisplayDialog("Font Scaler", "Couldn't scale this font:\n\n" + exception.Message, "OK");
                Debug.LogException(exception);
                return;
            }

            string dir = Path.GetDirectoryName(srcPath).Replace('\\', '/');
            string baseName = Path.GetFileNameWithoutExtension(srcPath) + SafeFileName(suffix);
            string outPath = $"{dir}/{baseName}.ttf";

            File.WriteAllBytes(outPath, scaled);
            AssetDatabase.ImportAsset(outPath, ImportAssetOptions.ForceUpdate);
            Font newFont = AssetDatabase.LoadAssetAtPath<Font>(outPath);

            UnityEngine.Object ping = newFont;
            if (createTmpAsset && newFont != null)
            {
                TMP_FontAsset fa = CreateOrRefreshTmpAsset(newFont, $"{dir}/{baseName} SDF.asset");
                if (fa != null)
                {
                    int pairs = ApplyKerningFromFontFile(fa, scaled);
                    Debug.Log($"[Font Scaler] Wrote {pairs} kerning pairs from the font file into {fa.name}.", fa);
                    ping = fa;
                }
            }

            foreach (string warning in warnings)
            {
                Debug.LogWarning("[Font Scaler] " + warning);
            }
            Debug.Log($"[Font Scaler] Wrote {outPath} (scale X {scale.x:0.###}, Y {scale.y:0.###}).", ping);
            EditorGUIUtility.PingObject(ping);
            Selection.activeObject = ping;
        }

        private static TMP_FontAsset CreateOrRefreshTmpAsset(Font font, string path)
        {
            TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (existing != null)
            {
                // Keep the existing asset (so references in scenes/prefabs survive) and clear its
                // dynamic data so glyphs are regenerated from the new font file.
                MethodInfo clear = typeof(TMP_FontAsset).GetMethod("ClearFontAssetData",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (clear != null)
                {
                    ParameterInfo[] parameters = clear.GetParameters();
                    clear.Invoke(existing, parameters.Length == 0 ? null : new object[] { true });
                }
                else
                {
                    Debug.LogWarning("[Font Scaler] Couldn't clear the existing TMP asset's data. Right-click it and choose Clear Dynamic Data / Update Atlas.");
                }
                EditorUtility.SetDirty(existing);
                AssetDatabase.SaveAssets();
                return existing;
            }

            TMP_FontAsset fa = TMP_FontAsset.CreateFontAsset(font);
            if (fa == null)
            {
                Debug.LogError("[Font Scaler] TextMeshPro couldn't create a font asset. Make sure the font's import settings have 'Include Font Data' enabled.");
                return null;
            }

            string name = Path.GetFileNameWithoutExtension(path);
            fa.name = name;
            AssetDatabase.CreateAsset(fa, path);

            fa.atlasTextures[0].name = name + " Atlas";
            AssetDatabase.AddObjectToAsset(fa.atlasTextures[0], fa);
            fa.material.name = name + " Material";
            AssetDatabase.AddObjectToAsset(fa.material, fa);

            EditorUtility.SetDirty(fa);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path);
            return fa;
        }

        /// <summary>
        /// TMP's automatic font-feature import can pull kerning/ligature data from the wrong font face
        /// (we've seen records pointing at glyph IDs the font doesn't even have, e.g. a bogus T→H kern).
        /// So: turn automatic feature import off, wipe the feature table, and write the pair kerning
        /// we read straight out of the font file ourselves.
        /// </summary>
        private static int ApplyKerningFromFontFile(TMP_FontAsset fa, byte[] fontBytes)
        {
            List<FontUtil.PairKern> kerning = FontUtil.ReadPairKerning(fontBytes);
            SerializedObject so = new SerializedObject(fa);

            SerializedProperty getFeatures = so.FindProperty("m_GetFontFeatures");
            if (getFeatures != null)
            {
                getFeatures.boolValue = false;
            }

            float pointSize = so.FindProperty("m_FaceInfo.m_PointSize").floatValue;
            float upem = so.FindProperty("m_FaceInfo.m_UnitsPerEM").intValue;
            float unitScale = upem > 0 ? pointSize / upem : 0f;

            SerializedProperty table = so.FindProperty("m_FontFeatureTable");
            if (table != null)
            {
                // clear every record list in the feature table (pairs, ligatures, mark positioning, ...)
                SerializedProperty it = table.Copy();
                SerializedProperty end = table.GetEndProperty();
                if (it.Next(true))
                {
                    while (!SerializedProperty.EqualContents(it, end))
                    {
                        if (it.isArray && it.propertyType == SerializedPropertyType.Generic)
                        {
                            it.ClearArray();
                        }
                        if (!it.Next(false))
                        {
                            break;
                        }
                    }
                }

                SerializedProperty pairs = table.FindPropertyRelative("m_GlyphPairAdjustmentRecords");
                if (pairs != null)
                {
                    pairs.arraySize = kerning.Count;
                    for (int i = 0; i < kerning.Count; i++)
                    {
                        SerializedProperty element = pairs.GetArrayElementAtIndex(i);
                        FontUtil.PairKern kp = kerning[i];
                        element.FindPropertyRelative("m_FirstAdjustmentRecord.m_GlyphIndex").longValue = kp.First;
                        element.FindPropertyRelative("m_FirstAdjustmentRecord.m_GlyphValueRecord.m_XPlacement").floatValue = kp.FirstXPlacement * unitScale;
                        element.FindPropertyRelative("m_FirstAdjustmentRecord.m_GlyphValueRecord.m_YPlacement").floatValue = 0f;
                        element.FindPropertyRelative("m_FirstAdjustmentRecord.m_GlyphValueRecord.m_XAdvance").floatValue = kp.FirstXAdvance * unitScale;
                        element.FindPropertyRelative("m_FirstAdjustmentRecord.m_GlyphValueRecord.m_YAdvance").floatValue = 0f;
                        element.FindPropertyRelative("m_SecondAdjustmentRecord.m_GlyphIndex").longValue = kp.Second;
                        element.FindPropertyRelative("m_SecondAdjustmentRecord.m_GlyphValueRecord.m_XPlacement").floatValue = kp.SecondXPlacement * unitScale;
                        element.FindPropertyRelative("m_SecondAdjustmentRecord.m_GlyphValueRecord.m_YPlacement").floatValue = 0f;
                        element.FindPropertyRelative("m_SecondAdjustmentRecord.m_GlyphValueRecord.m_XAdvance").floatValue = 0f;
                        element.FindPropertyRelative("m_SecondAdjustmentRecord.m_GlyphValueRecord.m_YAdvance").floatValue = 0f;
                        SerializedProperty flags = element.FindPropertyRelative("m_FeatureLookupFlags");
                        if (flags != null)
                        {
                            flags.intValue = 0;
                        }
                    }
                }
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            fa.ReadFontAssetDefinition(); // rebuild TMP's lookup dictionaries
            EditorUtility.SetDirty(fa);
            AssetDatabase.SaveAssets();
            return kerning.Count;
        }

        private static string SafeFileName(string value)
        {
            foreach (char invalid in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(invalid, '_');
            }
            return value;
        }
    }
}
