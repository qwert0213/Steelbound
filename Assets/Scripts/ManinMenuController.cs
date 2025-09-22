using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    #region Fields
    public Button startButton;
    public Button quitButton;
    #endregion
    #region Button Assignement
    void Start()
    {
        startButton = GameObject.Find("StartButton").GetComponent<Button>();
        quitButton = GameObject.Find("QuitButton").GetComponent<Button>();

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() => SceneManager.LoadScene("Level3"));
        Time.timeScale = 1f;

        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(() => Application.Quit());
    }
    #endregion
    #region Start
    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }
    #endregion
    #region Quit
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion
}
