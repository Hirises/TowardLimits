using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    public static void BuildCustom(){
        // 젠킨스 쉘 스크립트에서 넘겨주는 인자를 읽어옴
        string[] args = Environment.GetCommandLineArgs();
        string buildTarget = GetArg("-customBuildTarget", args);
        string outputPath = GetArg("-customOutputPath", args);

        BuildTarget target = BuildTarget.StandaloneWindows64;
        switch (buildTarget.ToLower())
        {
            case "windows": target = BuildTarget.StandaloneWindows64; break;
            case "android": target = BuildTarget.Android; break;
            case "ios": target = BuildTarget.iOS; break;
            case "macos": target = BuildTarget.StandaloneOSX; break;
            case "webgl": target = BuildTarget.WebGL; break;
            default: {
                Debug.LogError("Unknowen Build Target. Exit BuildScript");
                Application.Quit(1);
                break;
            }
        }

        BuildPlayerOptions options = new BuildPlayerOptions();

        options.locationPathName = outputPath;
        options.target = target;

        Build(options);
    }
    
    private static string GetArg(string name, string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1) return args[i + 1];
        }
        return null;
    }

    private static void Build(BuildPlayerOptions options){
        options.scenes = GetEnabledScenePaths();
        options.options = BuildOptions.None;

        Debug.Log("### Jenkins Build Start ###");
        BuildReport result = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = result.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("### Jenkins Build End ###");
            Debug.Log("Build Target: " + options.target);
            Debug.Log("Build Location: " + options.locationPathName);
            Debug.Log("Status: Succeeded");
        } else {
            Debug.Log("### Jenkins Build End ###");
            Debug.Log("Build Target: " + options.target);
            Debug.Log("Build Location: " + options.locationPathName);
            Debug.LogError("Status: Failed");
            EditorApplication.Exit(1);
        }
    }

    private static string[] GetEnabledScenePaths()
    {
        var scenes = EditorBuildSettings.scenes;
        var scenePaths = new System.Collections.Generic.List<string>();
        foreach (var scene in scenes)
        {
            if (scene.enabled) scenePaths.Add(scene.path);
        }
        return scenePaths.ToArray();
    }
}
