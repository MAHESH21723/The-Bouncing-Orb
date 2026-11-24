using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEngine;

// Sets the app icons: Android adaptive icon (background + foreground layers) and the default icon.
// Headless: Unity.exe -batchmode -quit -projectPath . -executeMethod IconSetup.Run
public static class IconSetup
{
    [MenuItem("Bouncing Orb/Apply App Icons")]
    public static void Run()
    {
        var full = Load("Assets/Icon/AppIcon.png");
        var background = Load("Assets/Icon/AdaptiveBackground.png");
        var foreground = Load("Assets/Icon/AdaptiveForeground.png");

        PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new[] { full }, IconKind.Any);

        var target = NamedBuildTarget.Android;
        var adaptive = PlayerSettings.GetPlatformIcons(target, AndroidPlatformIconKind.Adaptive);
        foreach (var icon in adaptive)
        {
            icon.SetTexture(background, 0);
            icon.SetTexture(foreground, 1);
        }
        PlayerSettings.SetPlatformIcons(target, AndroidPlatformIconKind.Adaptive, adaptive);

        PlayerSettings.bundleVersion = "1.0.1";
        PlayerSettings.Android.bundleVersionCode = 2;
        AssetDatabase.SaveAssets();
        Debug.Log("ICONSETUP_DONE adaptive=" + adaptive.Length);
    }

    static Texture2D Load(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Default;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
}
