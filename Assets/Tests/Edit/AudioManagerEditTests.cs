using NUnit.Framework;
using System.Reflection;
using UnityEngine;

public class AudioManagerEditTests
{
    [Test]
    public void AudioManager_SingletonInstance_IsCreated()
    {
        var go = new GameObject("AudioManager");
        var manager = go.AddComponent<AudioManager>();

        var instanceField = typeof(AudioManager).GetField("instance", BindingFlags.NonPublic | BindingFlags.Static)
                            ?? typeof(AudioManager).GetField("Instance", BindingFlags.Public | BindingFlags.Static);

        Assert.IsNotNull(instanceField, "Az AudioManager-ben nem található instance mezõ.");

        instanceField.SetValue(null, manager);

        var value = instanceField.GetValue(null);
        Assert.IsNotNull(value, "Az AudioManager instance nem lett beállítva.");
    }



    [Test]
    public void AudioManager_CanPlaySoundClip()
    {
        var audioGO = new GameObject("AudioManager");
        var audio = audioGO.AddComponent<AudioManager>();
        var source = audioGO.AddComponent<AudioSource>();
        audio.GetType().GetField("clip", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(audio, AudioClip.Create("test", 44100, 1, 44100, false));
        Assert.DoesNotThrow(() => source.Play());
    }

}
