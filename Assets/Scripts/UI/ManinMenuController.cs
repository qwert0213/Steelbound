using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;

    void Start()
    {
        startButton = GameObject.Find("StartButton").GetComponent<Button>();
        quitButton = GameObject.Find("QuitButton").GetComponent<Button>();

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() => SceneManager.LoadScene("Level 1"));

        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(() => Application.Quit());
    }


public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
