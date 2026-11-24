using UnityEditor;
using UnityEngine;

/// Builds the Android APK into Builds/BouncingOrb.apk.
/// Headless: Unity.exe -batchmode -quit -projectPath . -buildTarget Android -executeMethod AndroidBuild.Build
public static class AndroidBuild
{
    [MenuItem("Bouncing Orb/Build Android APK")]
    public static void Build()
    {
        EditorUserBuildSettings.buildAppBundle = false;
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/SampleScene.unity" },
            target = BuildTarget.Android,
            locationPathName = "Builds/BouncingOrb.apk",
            options = BuildOptions.None
        });
        Debug.Log("ORB_BUILD " + report.summary.result + " " + report.summary.totalSize);
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) EditorApplication.Exit(2);
    }
}
