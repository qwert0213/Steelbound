using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    #region Fields
    public CoinCount coinCounter;       
    public GameObject levelEndMenu;    
    public int coinsToFinish = 70;
    #endregion
    #region Base
    private void Start()
    {
        if (levelEndMenu != null)
            levelEndMenu.SetActive(false);
    }
    #endregion
    #region Completion
    private void Update()
    {
        if (coinCounter != null && coinCounter.Count >= coinsToFinish)
        {
            LevelComplete();
        }
    }

    private void LevelComplete()
    {
        if (levelEndMenu != null)
            levelEndMenu.SetActive(true);

        Time.timeScale = 0f;  
    }
    #endregion
    #region Buttons
    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
    #endregion
}
