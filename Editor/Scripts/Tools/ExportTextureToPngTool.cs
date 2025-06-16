using UnityEngine;
using UnityEditor;
using System.IO;

public static class ExportTextureToPngTool
{
    [MenuItem("Assets/Tools/Export Texture to PNG", true)]
    private static bool ValidateTexture()
    {
        return Selection.activeObject is Texture2D;
    }

    [MenuItem("Assets/Tools/Export Texture to PNG")]
    private static void Export()
    {
        Texture2D texture = Selection.activeObject as Texture2D;

        if (texture == null)
        {
            Debug.LogError("No Texture2D selected.");
            return;
        }

        byte[] pngData = texture.EncodeToPNG();
        if (pngData == null)
        {
            Debug.LogError("Failed to encode texture as PNG.");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(texture);
        string assetDirectory = Path.GetDirectoryName(assetPath);
        string fileName = Path.GetFileNameWithoutExtension(assetPath);
        string exportPath = Path.Combine(assetDirectory, fileName + "_sliced.png");

        File.WriteAllBytes(exportPath, pngData);
        AssetDatabase.Refresh();

        Debug.Log($"✅ Saved: {exportPath}");
    }
}
