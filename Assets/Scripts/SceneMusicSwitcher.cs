using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusicSwitcher : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
            AudioManager.Instance.PlayMusic(AudioManager.Instance.menuMusic);
        else if (scene.name == "Level1")
            AudioManager.Instance.PlayMusic(AudioManager.Instance.levelMusic);
    }
}
