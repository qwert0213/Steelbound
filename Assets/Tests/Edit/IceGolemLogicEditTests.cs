using NUnit.Framework;
using UnityEngine;

public class IceGolemLogicEditTests
{
    private GameObject golemObj;
    private IceGolemLogic golem;

    [SetUp]
    public void Setup()
    {
        golemObj = new GameObject("IceGolem");
        golem = golemObj.AddComponent<IceGolemLogic>();
    }

    [Test]
    public void IceGolem_CanDamagePlayer_IsTrueInitially()
    {
        var field = typeof(IceGolemLogic).GetField("canDamagePlayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bool value = (bool)field.GetValue(golem);
        Assert.IsTrue(value, "Az IceGolemnek kezdetben képesnek kell lennie sebzést okozni.");
    }

    [Test]
    public void IceGolem_PlayerDamageCooldown_DefaultIsZero()
    {
        var field = typeof(IceGolemLogic).GetField("playerDamageCooldown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        float value = (float)field.GetValue(golem);
        Assert.AreEqual(0f, value, "A cooldownnak kezdetben nullának kell lennie.");
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(golemObj);
    }
}
