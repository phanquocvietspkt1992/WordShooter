using System.IO;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PrototypeSceneSetup
{
    private const string ScenePath = "Assets/Scenes/Prototype.unity";

    static PrototypeSceneSetup()
    {
        EditorApplication.delayCall += InitializeEditor;
    }

    private static void InitializeEditor()
    {
        EnsurePrototypeScene();
        ShowPhonePreview();
    }

    public static void CreatePrototypeScene()
    {
        EnsurePrototypeScene();
    }

    [MenuItem("Word Shooter/Phone Portrait Preview")]
    public static void ShowPhonePreview()
    {
        var assembly = typeof(Editor).Assembly;
        var sizesType = assembly.GetType("UnityEditor.GameViewSizes", true);
        var singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var sizes = singleton.GetProperty("instance").GetValue(null);
        var groupType = sizesType.GetProperty("currentGroupType").GetValue(sizes);
        var group = sizesType.GetMethod("GetGroup").Invoke(sizes, new[] { groupType });
        var index = FindPhoneSize(group);
        var view = EditorWindow.GetWindow(assembly.GetType("UnityEditor.GameView", true));
        view.GetType().GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .SetValue(view, index);
        view.Focus();
        view.Repaint();
        Debug.Log("Word Galaxy phone preview selected: 390 x 844 portrait.");
    }

    private static int FindPhoneSize(object group)
    {
        var groupType = group.GetType();
        var count = (int)groupType.GetMethod("GetTotalCount").Invoke(group, null);
        for (var index = 0; index < count; index++)
        {
            var size = groupType.GetMethod("GetGameViewSize").Invoke(group, new object[] { index });
            if ((int)size.GetType().GetProperty("width").GetValue(size) == 390
                && (int)size.GetType().GetProperty("height").GetValue(size) == 844) return index;
        }
        var assembly = typeof(Editor).Assembly;
        var sizeType = assembly.GetType("UnityEditor.GameViewSize", true);
        var resolutionType = assembly.GetType("UnityEditor.GameViewSizeType", true);
        var phone = Activator.CreateInstance(sizeType, Enum.Parse(resolutionType, "FixedResolution"),
            390, 844, "Phone Portrait");
        groupType.GetMethod("AddCustomSize").Invoke(group, new[] { phone });
        return count;
    }

    private static void EnsurePrototypeScene()
    {
        if (!File.Exists(ScenePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, ScenePath);
        }

        var buildScenes = EditorBuildSettings.scenes;
        var sceneIsRegistered = false;
        foreach (var buildScene in buildScenes)
        {
            if (buildScene.path == ScenePath && buildScene.enabled)
            {
                sceneIsRegistered = true;
                break;
            }
        }

        if (!sceneIsRegistered)
        {
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
    }
}