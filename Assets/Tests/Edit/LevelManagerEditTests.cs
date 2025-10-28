using NUnit.Framework;
using UnityEngine;

public class LevelManagerEditTests
{
    private LevelManager manager;
    private CoinCount coinCount;

    [SetUp]
    public void Setup()
    {
        manager = new GameObject("LevelManager").AddComponent<LevelManager>();
        coinCount = new GameObject("CoinCount").AddComponent<CoinCount>();
        manager.coinCounter = coinCount;
        manager.coinsToFinish = 4;
    }

    [Test]
    public void LevelComplete_WhenEnoughCoins()
    {
        coinCount.AddQuantity(5);
        bool isComplete = coinCount.Count >= manager.coinsToFinish;
        Assert.IsTrue(isComplete, "A szintnek befejezettnek kell lennie, ha a szükséges érmeszám megvan.");
    }

    [Test]
    public void LevelNotComplete_WhenNotEnoughCoins()
    {
        coinCount.AddQuantity(3);
        bool isComplete = coinCount.Count >= manager.coinsToFinish;
        Assert.IsFalse(isComplete);
    }
}
