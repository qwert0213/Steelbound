using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    #region Fields
    public GameObject pauseMenuUI;
    public MonoBehaviour[] scriptsToDisable; 
    private bool isPaused = false;

    #endregion

    #region Default
    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    #endregion

    #region Continue
    public void ResumeGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        SetPlayerScriptsActive(true);
        isPaused = false;
    }
    #endregion

    #region Restart
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion

    #region Main Menu
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    #endregion

    #region Pause
    private void PauseGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        SetPlayerScriptsActive(false);
        isPaused = true;
    }
    #endregion

    #region Helpers
    private void SetPlayerScriptsActive(bool state)
    {
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = state;
        }
    }
    #endregion
}
