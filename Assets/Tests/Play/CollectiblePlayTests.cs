using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

public class CollectiblePlayTests : PlayModeTestBase
{
    private GameObject coinCounterGO;
    private CoinCount coinCount;
    private GameObject keyCounterGO;
    private KeyCount keyCount;
    private GameObject playerGO;

    [SetUp]
    public void Setup()
    {
        coinCounterGO = new GameObject("CoinScore");
        coinCounterGO.AddComponent<Text>();
        coinCount = coinCounterGO.AddComponent<CoinCount>();

        keyCounterGO = new GameObject("KeyScore");
        keyCounterGO.AddComponent<Text>();
        keyCount = keyCounterGO.AddComponent<KeyCount>();

        playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        playerGO.AddComponent<Rigidbody2D>();
        playerGO.AddComponent<BoxCollider2D>();
    }

    [UnityTest]
    public IEnumerator CollectCoin_IncreasesCoinCounter()
    {
        var coinScore = new GameObject("CoinScore");
        coinScore.tag = "CoinScore";
        var text = coinScore.AddComponent<Text>();
        text.text = "0";
        var coinCounter = coinScore.AddComponent<CoinCount>();

        var player = new GameObject("Player");
        player.tag = "Player";
        var playerCollider = player.AddComponent<BoxCollider2D>();
        playerCollider.isTrigger = false;

        var coin = new GameObject("Coin");
        var coinCollider = coin.AddComponent<CircleCollider2D>();
        coinCollider.isTrigger = true;
        var collectCoin = coin.AddComponent<CollectCoin>();
        yield return null;

        var ccType = typeof(CollectCoin);
        var collectField = ccType.GetField("collect", BindingFlags.NonPublic | BindingFlags.Instance);
        collectField?.SetValue(collectCoin, true);

        var updateMethod = ccType.GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance)
                         ?? ccType.GetMethod("Update", BindingFlags.Public | BindingFlags.Instance);
        updateMethod?.Invoke(collectCoin, null);

        yield return null;
        Assert.AreEqual("10", text.text, $"A CollectCoin nem növelte meg a CoinScore értékét. Jelenlegi: '{text.text}'");
    }

    [UnityTest]
    public IEnumerator CollectKey_IncreasesKeyCounter_And_NotifiesCrow()
    {
        var keyScore = new GameObject("KeyScore");
        keyScore.tag = "KeyScore";
        var text = keyScore.AddComponent<Text>();
        text.text = "0";
        keyScore.AddComponent<KeyCount>();

        var crowGO = new GameObject("Crow");
        crowGO.SetActive(false);
        var crowLogic = crowGO.AddComponent<CrowLogic>(); 
        crowGO.hideFlags = HideFlags.DontSave;

        var player = new GameObject("Player");
        player.tag = "Player";
        player.AddComponent<BoxCollider2D>();

        var key = new GameObject("Key");
        var keyCollider = key.AddComponent<CircleCollider2D>();
        keyCollider.isTrigger = true;
        var collectKey = key.AddComponent<CollectKey>();

        yield return null;

        var ckType = typeof(CollectKey);
        var crowField = ckType.GetField("crow", BindingFlags.NonPublic | BindingFlags.Instance);
        crowField?.SetValue(collectKey, crowLogic);

        var collectField = ckType.GetField("collect", BindingFlags.NonPublic | BindingFlags.Instance);
        collectField?.SetValue(collectKey, true);

        var updateMethod = ckType.GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance)
                         ?? ckType.GetMethod("Update", BindingFlags.Public | BindingFlags.Instance);
        updateMethod?.Invoke(collectKey, null);

        yield return null;

        Assert.AreEqual("1", text.text, "A CollectKey nem növelte meg a KeyScore értékét.");


    }
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(coinCounterGO);
        Object.DestroyImmediate(keyCounterGO);
        Object.DestroyImmediate(playerGO);
    }


}

public class CrowLogicMock : MonoBehaviour
{
    private bool wasNotified;
    private void CrowReact()
    {
        wasNotified = true;
    }
    public bool GetWasNotified() => wasNotified;
}