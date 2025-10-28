using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.TestTools;

public class PlayModeTestBase
{
    [SetUp]
    public void Init()
    {
        SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
        Time.timeScale = 1f;
    }

    [UnityTearDown]
    public IEnumerator Clean()
    {
        var objects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in objects)
        {
            if (obj != null)
                Object.Destroy(obj);
        }
        yield return null;
    }  
}
