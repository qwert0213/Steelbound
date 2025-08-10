using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    #region Fields
    public Slider volumeSlider;
    #endregion

    #region Default
    void Start()
    {
        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("gameVolume", 1f);
            volumeSlider.value = savedVolume;
            AudioListener.volume = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }
    #endregion

    #region SetVolume
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("gameVolume", volume);
    }
    #endregion
}
