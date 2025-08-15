using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Fields
    public static AudioManager Instance;
    private AudioSource loopSFXSource;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip levelMusic;

    [Header("Player SFX")]
    public AudioClip playerAttack;
    public AudioClip playerHurt;
    public AudioClip playerWalk;
    public AudioClip playerDeath;

    [Header("Enemy SFX")]
    public AudioClip mushroomAttack;
    public AudioClip mushroomHurt;
    public AudioClip mushroomDeath;
    public AudioClip crowFly;
    public AudioClip crowSay;
    public AudioClip wolfAttack;
    public AudioClip wolfHurt;
    public AudioClip wolfDeath;
    public AudioClip iceGolemDeath;
    public AudioClip iceGolemRun;


    private AudioSource musicSource;
    private AudioSource sfxSource;
    #endregion
    #region Singleton
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;

            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Music
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.Play();
    }
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    #endregion
    #region Effects
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    #endregion
    #region Loop
    public void PlayLoopSFX(AudioClip clip)
    {
        if (loopSFXSource == null)
        {
            loopSFXSource = gameObject.AddComponent<AudioSource>();
            loopSFXSource.loop = true;
        }

        if (loopSFXSource.clip == clip && loopSFXSource.isPlaying)
            return;

        loopSFXSource.clip = clip;
        loopSFXSource.Play();
    }
    public void StopLoopSFX()
    {
        if (loopSFXSource != null && loopSFXSource.isPlaying)
            loopSFXSource.Stop();
    }
    #endregion
}
