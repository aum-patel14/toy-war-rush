#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public static class ToyWarRushBuild
{
    private const string WebGLOutput = "Build/WebGL";
    private const string AndroidOutput = "Build/Android/ToyWarRush.apk";

    [MenuItem("ToyWarRush/Build WebGL (Play in Browser)")]
    public static void BuildWebGLMenu()
    {
        var report = BuildWebGL();
        if (!Application.isBatchMode && report.summary.result == BuildResult.Succeeded)
            EditorUtility.RevealInFinder(WebGLOutput);
    }

    [MenuItem("ToyWarRush/Build Android APK")]
    public static void BuildAndroidMenu()
    {
        var report = BuildAndroid();
        if (!Application.isBatchMode && report.summary.result == BuildResult.Succeeded)
            EditorUtility.RevealInFinder(Path.GetDirectoryName(AndroidOutput));
    }

    public static void BuildWebGLBatch()
    {
        var report = BuildWebGL();
        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError($"[ToyWarRush] WebGL build failed: {report.summary.result}");
            EditorApplication.Exit(1);
        }
        else
        {
            Debug.Log("[ToyWarRush] WebGL build succeeded: " + WebGLOutput);
            EditorApplication.Exit(0);
        }
    }

    public static void BuildAndroidBatch()
    {
        var report = BuildAndroid();
        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError($"[ToyWarRush] Android build failed: {report.summary.result}");
            EditorApplication.Exit(1);
        }
        else
        {
            Debug.Log("[ToyWarRush] Android build succeeded: " + AndroidOutput);
            EditorApplication.Exit(0);
        }
    }

    public static BuildReport BuildWebGL()
    {
        SetupScenes();
        if (Directory.Exists(WebGLOutput))
            Directory.Delete(WebGLOutput, true);

        return BuildPipeline.BuildPlayer(
            EditorBuildSettings.scenes,
            WebGLOutput,
            BuildTarget.WebGL,
            BuildOptions.None);
    }

    public static BuildReport BuildAndroid()
    {
        SetupScenes();
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

        var dir = Path.GetDirectoryName(AndroidOutput);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        return BuildPipeline.BuildPlayer(
            EditorBuildSettings.scenes,
            AndroidOutput,
            BuildTarget.Android,
            BuildOptions.Development);
    }

    private static void SetupScenes()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/_Game/Scenes/Boot.unity", true),
            new EditorBuildSettingsScene("Assets/_Game/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/_Game/Scenes/Gameplay.unity", true),
        };
    }
}
#endif
