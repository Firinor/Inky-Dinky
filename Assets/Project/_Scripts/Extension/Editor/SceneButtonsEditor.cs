using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

public static class SceneButtonsEditor
{
    [MainToolbarElement("MyTools/SceneBootstrap", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement CreateSceneBootstrapButton()
    {
        var icon = EditorGUIUtility.IconContent("Visible").image as Texture2D;
        var content = new MainToolbarContent(icon, "To scene");
        
        return new MainToolbarButton(content, () => {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene("Assets/Scenes/BOOTSTRAP.unity", OpenSceneMode.Single);
        });
    }
    [MainToolbarElement("MyTools/SceneMeta", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement CreateSceneMetaButton()
    {
        var icon = EditorGUIUtility.IconContent("Unlocked").image as Texture2D;
        var content = new MainToolbarContent(icon, "To scene");
        
        return new MainToolbarButton(content, () => {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene("Assets/Scenes/WorldMapScene.unity", OpenSceneMode.Single);
        });
    }
    [MainToolbarElement("MyTools/SceneCore", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement CreateSceneCoreButton()
    {
        var icon = EditorGUIUtility.IconContent("console.infoicon").image as Texture2D;
        var content = new MainToolbarContent(icon, "To scene");
        
        return new MainToolbarButton(content, () => {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity", OpenSceneMode.Single);
        });
    }
}
