using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PlayFromMainMenu
{
    static PlayFromMainMenu()
    {
        EditorApplication.playModeStateChanged += LoadMainMenuOnPlay;
    }

    private static void LoadMainMenuOnPlay(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Útvonal a Main Menu scene-re
            string mainMenuPath = "Assets/Scenes/MainMenu.unity";

            if (EditorSceneManager.GetActiveScene().path != mainMenuPath)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(mainMenuPath);
                }
                else
                {
                    EditorApplication.isPlaying = false;
                }
            }
        }
    }
}
