using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    #region Fields
    public CoinCount coinCounter;
    public GameObject levelEndMenu;
    public GameObject gameCompleteMenu;
    public PlayerMovement pm;
    public int coinsToFinish = 70;
    [Header("Cutscene")]
    public GameObject crowPrefab;
    public float cutsceneDelay = 2f;
    private bool levelEnding = false; 

    #endregion

    #region Base
    private void Start()
    {
        if (levelEndMenu != null)
            levelEndMenu.SetActive(false);

        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        switch (currentLevel)
        {
            case 1: 
                coinsToFinish = 70;
                break;
            case 2: 
                coinsToFinish = 60;
                if (pm != null)
                    pm.isSlippery = true;
                break;
            case 3:
                coinsToFinish = 60;
                if (pm != null)
                    pm.canWallCling = true;
                break;

        }
    }
    #endregion

    #region Completion
    private void Update()
    {
        if (!levelEnding && coinCounter != null && coinCounter.Count >= coinsToFinish)
        {
            LevelComplete();
        }
    }

    private void LevelComplete()
    {
        levelEnding = true;
        StartCoroutine(EndLevelSequence());
    }

    private IEnumerator EndLevelSequence()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Vector3 spawnPos = pm.transform.position + new Vector3(0, 15f, 0);
            GameObject crowObj = Instantiate(crowPrefab, spawnPos, Quaternion.identity);
            CrowLogic crow = crowObj.GetComponent<CrowLogic>();
            if (crow != null)
                crow.state = CrowLogic.CrowState.GoToPlayer;
            yield return new WaitForSeconds(cutsceneDelay);
            Destroy(crowObj);
        }

        int currentLevel = SceneManager.GetActiveScene().buildIndex;

        if (currentLevel != 3)
        {
            StoryManager.Instance.PlayLevelOutro(() =>
            {
                levelEndMenu.SetActive(true);
                Time.timeScale = 0f;
            });
        }
        else
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.alarmClock);
            StoryManager.Instance.PlayLevelOutro(() =>
            {
                gameCompleteMenu.SetActive(true);
                Time.timeScale = 0f;
                AudioManager.Instance.PlayMusic(AudioManager.Instance.menuMusic); 
            });
        }
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
