using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider volumeSlider;
    public Text attackButtonText;
    public Text blockButtonText;
    public Text rollButtonText;
    public Text interactButtonText;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    private string waitingForKey = null;
    private KeyCode originalKey;
    private bool waitingForKeyUp = false;
    private float keybindCooldown = 0f;
    private float keybindCooldownDuration = 1f;

    void Start()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        volumeSlider.value = SettingsManager.Instance.GetVolume();
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        UpdateUI();
    }

    void Update()
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
                            SettingsManager.Instance.SetKeyForAction(waitingForKey, originalKey);
                        }
                        else
                        {
                            SettingsManager.Instance.SetKeyForAction(waitingForKey, key);
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
            if (Input.GetKey(key))
                return false;
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
            return false;
        return true;
    }

    public void StartKeyBinding(string action)
    {
        if (Time.time < keybindCooldown)
            return;

        waitingForKey = action;
        originalKey = SettingsManager.Instance.GetKeyForAction(action);
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

    public void BackToMainMenu()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void Settings()
    {
        settingsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    private void UpdateUI()
    {
        attackButtonText.text = SettingsManager.Instance.GetKeyForAction("Attack").ToString();
        blockButtonText.text = SettingsManager.Instance.GetKeyForAction("Block").ToString();
        rollButtonText.text = SettingsManager.Instance.GetKeyForAction("Roll").ToString();
        interactButtonText.text = SettingsManager.Instance.GetKeyForAction("Interact").ToString();
        volumeSlider.value = SettingsManager.Instance.GetVolume();
    }

    private void OnVolumeChanged(float value)
    {
        SettingsManager.Instance.SetVolume(value);
    }

}

