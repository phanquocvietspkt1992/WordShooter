using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WordShooterMobileBuild
{
    public static void ExportIOS()
    {
        Build(BuildTarget.iOS);
    }

    public static void BuildAndroidBundle()
    {
        Build(BuildTarget.Android);
    }

    private static void Build(BuildTarget target)
    {
        if (!BuildPipeline.IsBuildTargetSupported(BuildPipeline.GetBuildTargetGroup(target), target))
            throw new InvalidOperationException("Install Unity Build Support for " + target + " before building.");
        if (EditorUserBuildSettings.activeBuildTarget != target)
            throw new InvalidOperationException("Start Unity with -buildTarget " + target + " before building.");

        var namedTarget = target == BuildTarget.iOS ? NamedBuildTarget.iOS : NamedBuildTarget.Android;
        if (PlayerSettings.GetScriptingBackend(namedTarget) != ScriptingImplementation.IL2CPP)
            throw new InvalidOperationException("Mobile release builds must use IL2CPP.");
        if (target == BuildTarget.Android
            && ((int)PlayerSettings.Android.targetSdkVersion < 36
                || PlayerSettings.Android.targetArchitectures != AndroidArchitecture.ARM64))
            throw new InvalidOperationException("Android builds require explicit target API 36 or newer and ARM64.");

        var args = Environment.GetCommandLineArgs();
        var outputIndex = Array.IndexOf(args, "-wordShooterOutput");
        if (outputIndex < 0 || outputIndex + 1 >= args.Length
            || args[outputIndex + 1].StartsWith("-", StringComparison.Ordinal))
            throw new ArgumentException("Supply an output path with -wordShooterOutput.");
        var output = Path.GetFullPath(args[outputIndex + 1]);
        var assetsPath = Path.GetFullPath(Application.dataPath);
        var projectPath = Path.GetDirectoryName(assetsPath);
        if (output == projectPath
            || projectPath.StartsWith(output.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar,
                StringComparison.Ordinal))
            throw new ArgumentException("Build output cannot be the project directory or one of its parents.");
        if (output == assetsPath || output.StartsWith(assetsPath + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            throw new ArgumentException("Build output must be outside Assets.");
        if (target == BuildTarget.Android && !output.EndsWith(".aab", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Android bundle output must end in .aab.");

        var scenes = Array.FindAll(EditorBuildSettings.scenes, scene => scene.enabled);
        if (scenes.Length == 0) throw new InvalidOperationException("No enabled build scenes.");

        var previousAppBundle = EditorUserBuildSettings.buildAppBundle;
        var signing = target == BuildTarget.Android ? AndroidSigning.FromEnvironment() : null;
        var previousSigning = target == BuildTarget.Android ? AndroidSigning.Capture() : null;
        try
        {
            if (target == BuildTarget.Android)
            {
                EditorUserBuildSettings.buildAppBundle = true;
                ConfigureAndroidDebugSymbols();
                if (signing != null) signing.Apply();
                else Debug.LogWarning("WordShooter: no upload key in the environment; the bundle is debug-signed and "
                    + "cannot be uploaded to Google Play. Set " + AndroidSigning.EnvironmentNames + ".");
            }
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = Array.ConvertAll(scenes, scene => scene.path),
                locationPathName = output,
                target = target,
                options = BuildOptions.None
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Mobile build failed: " + report.summary.result
                    + " (" + report.summary.totalErrors + " errors). See the Unity build log.");
            Debug.Log("WordShooter release build succeeded: " + output
                + " (" + report.summary.totalSize + " bytes). Signing and store validation are separate steps.");
        }
        finally
        {
            EditorUserBuildSettings.buildAppBundle = previousAppBundle;
            if (previousSigning != null) previousSigning.Apply();
        }
    }

    private static void ConfigureAndroidDebugSymbols()
    {
#if UNITY_ANDROID
        // Play Console needs native symbols to symbolicate crashes and ANRs.
        UnityEditor.Android.UserBuildSettings.DebugSymbols.level = Unity.Android.Types.DebugSymbolLevel.SymbolTable;
        UnityEditor.Android.UserBuildSettings.DebugSymbols.format = Unity.Android.Types.DebugSymbolFormat.IncludeInBundle;
#endif
    }

    private sealed class AndroidSigning
    {
        public const string EnvironmentNames = "WORDSHOOTER_KEYSTORE, WORDSHOOTER_KEYSTORE_PASS, "
            + "WORDSHOOTER_KEY_ALIAS, and WORDSHOOTER_KEY_PASS";

        private bool useCustomKeystore;
        private string keystoreName;
        private string keystorePass;
        private string keyaliasName;
        private string keyaliasPass;

        public static AndroidSigning FromEnvironment()
        {
            var keystore = Environment.GetEnvironmentVariable("WORDSHOOTER_KEYSTORE");
            if (string.IsNullOrEmpty(keystore)) return null;
            var signing = new AndroidSigning
            {
                useCustomKeystore = true,
                keystoreName = Path.GetFullPath(keystore),
                keystorePass = Environment.GetEnvironmentVariable("WORDSHOOTER_KEYSTORE_PASS"),
                keyaliasName = Environment.GetEnvironmentVariable("WORDSHOOTER_KEY_ALIAS"),
                keyaliasPass = Environment.GetEnvironmentVariable("WORDSHOOTER_KEY_PASS")
            };
            if (!File.Exists(signing.keystoreName))
                throw new FileNotFoundException("WORDSHOOTER_KEYSTORE does not exist.", signing.keystoreName);
            if (string.IsNullOrEmpty(signing.keystorePass) || string.IsNullOrEmpty(signing.keyaliasName)
                || string.IsNullOrEmpty(signing.keyaliasPass))
                throw new ArgumentException("Set all of " + EnvironmentNames + " to sign the bundle.");
            return signing;
        }

        public static AndroidSigning Capture()
        {
            return new AndroidSigning
            {
                useCustomKeystore = PlayerSettings.Android.useCustomKeystore,
                keystoreName = PlayerSettings.Android.keystoreName,
                keystorePass = PlayerSettings.Android.keystorePass,
                keyaliasName = PlayerSettings.Android.keyaliasName,
                keyaliasPass = PlayerSettings.Android.keyaliasPass
            };
        }

        public void Apply()
        {
            PlayerSettings.Android.useCustomKeystore = useCustomKeystore;
            PlayerSettings.Android.keystoreName = keystoreName;
            PlayerSettings.Android.keystorePass = keystorePass;
            PlayerSettings.Android.keyaliasName = keyaliasName;
            PlayerSettings.Android.keyaliasPass = keyaliasPass;
        }
    }
}
