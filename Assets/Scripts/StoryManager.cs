using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    #region Fields
    public static StoryManager Instance;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public PlayerMovement player;
    public Text dialogueText;  

    #endregion
    #region Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    #region Intro/Outro
    private void Start()
    {
        PlayLevelIntro();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Level"))
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            PlayLevelIntro();
        }
    }
    public void PlayLevelIntro()
    {
        int levelIndex = GetCurrentLevelIndex();
        string[] lines = StoryDatabase.GetIntroLines(levelIndex);
        if (lines != null) StartCoroutine(PlayDialogueSequence(lines));
    }

    public void PlayLevelOutro(System.Action onComplete = null)
    {
        int levelIndex = GetCurrentLevelIndex();
        string[] lines = StoryDatabase.GetOutroLines(levelIndex);
        if (lines != null) StartCoroutine(PlayDialogueSequence(lines, onComplete));
    }
    #endregion

    #region Dialog
    private IEnumerator PlayDialogueSequence(string[] lines, System.Action onComplete = null)
    {
        dialoguePanel.SetActive(true);
        player.controllable = false;
        foreach (string line in lines)
        {
            dialogueText.text = line;
            yield return new WaitForSeconds(3f);
        }
        player.controllable = true;
        dialoguePanel.SetActive(false);
        onComplete?.Invoke();
    }
    #endregion
    #region Level
    private int GetCurrentLevelIndex()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    }
    #endregion
}
