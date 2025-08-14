using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    #region Singleton
    public static SettingsManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDefaults();
            AudioListener.volume = volume;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Fields
    [Header("UI References")]
    public Slider volumeSlider;
    public Text attackButtonText;
    public Text blockButtonText;
    public Text rollButtonText;
    public Text interactButtonText;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    private Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();
    private Dictionary<string, KeyCode> defaultBindings = new Dictionary<string, KeyCode>()
    {
        {"Attack", KeyCode.Mouse0},
        {"Block", KeyCode.Mouse1},
        {"Roll", KeyCode.LeftShift},
        {"Interact", KeyCode.E}
    };

    private string waitingForKey = null;
    private KeyCode originalKey;
    private bool waitingForKeyUp = false;
    private float keybindCooldown = 0f;
    private readonly float keybindCooldownDuration = 1f;

    private float volume = 1f;
    #endregion

    #region Initialization
    private void InitializeDefaults()
    {
        foreach (var action in defaultBindings.Keys)
            keyBindings[action] = defaultBindings[action];
    }

    private void Start()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        UpdateUI();
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(SetVolume);
    }
    #endregion

    #region Keybinding
    private void Update()
    {
        if (waitingForKey != null)
        {
            if (waitingForKeyUp)
            {
                if (NoKeyIsPressed())
                    waitingForKeyUp = false;
            }
            else
            {
                foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        if (key == KeyCode.Escape)
                        {
                            keyBindings[waitingForKey] = originalKey;
                        }
                        else
                        {
                            keyBindings[waitingForKey] = key;
                        }
                        waitingForKey = null;
                        keybindCooldown = Time.time + keybindCooldownDuration;
                        UpdateUI();
                        break;
                    }
                }
            }
        }
    }

    private bool NoKeyIsPressed()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            if (Input.GetKey(key)) return false;

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
            return false;

        return true;
    }

    public void StartKeyBinding(string action)
    {
        if (Time.time < keybindCooldown)
            return;

        waitingForKey = action;
        originalKey = keyBindings[action];
        waitingForKeyUp = true;

        switch (action)
        {
            case "Attack":
                attackButtonText.text = "Press any key...";
                break;
            case "Block":
                blockButtonText.text = "Press any key...";
                break;
            case "Roll":
                rollButtonText.text = "Press any key...";
                break;
            case "Interact":
                interactButtonText.text = "Press any key...";
                break;
        }
    }
    #endregion

    #region UI
    private void UpdateUI()
    {
        if (attackButtonText != null) attackButtonText.text = keyBindings["Attack"].ToString();
        if (blockButtonText != null) blockButtonText.text = keyBindings["Block"].ToString();
        if (rollButtonText != null) rollButtonText.text = keyBindings["Roll"].ToString();
        if (interactButtonText != null) interactButtonText.text = keyBindings["Interact"].ToString();

        if (volumeSlider != null) volumeSlider.value = volume;

        AudioListener.volume = volume;
    }

    public void SetVolume(float value)
    {
        volume = value;
        AudioListener.volume = volume;
    }
    #endregion

    #region Panels
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
    #endregion

    #region Getters Setters
    public KeyCode GetKeyForAction(string action)
    {
        if (keyBindings.ContainsKey(action))
            return keyBindings[action];
        return KeyCode.None;
    }

    public void SetKeyForAction(string action, KeyCode key)
    {
        keyBindings[action] = key;
        UpdateUI();
    }

    public float GetVolume()
    {
        return volume;
    }
    #endregion
}
