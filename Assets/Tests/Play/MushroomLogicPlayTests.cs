using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MushroomLogicPlayTests : PlayModeTestBase
{
    private GameObject mushObj;
    private MushroomLogic mush;
    private GameObject playerGO;

    [SetUp]
    public void Setup()
    {
        playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        playerGO.AddComponent<SafePlayerStub>();

        mushObj = new GameObject("Mushroom");
        mush = mushObj.AddComponent<MushroomLogic>();
    }

    [UnityTest]
    public IEnumerator Awake_Initializes_Mushroom_And_Is_Damageable()
    {
        yield return null;

        var healthField = typeof(EnemyLogic).GetField("health", BindingFlags.NonPublic | BindingFlags.Instance);
        var damageableField = typeof(EnemyLogic).GetField("damageable", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(healthField, "EnemyLogic.health field not found.");
        Assert.IsNotNull(damageableField, "EnemyLogic.damageable field not found.");

        float health = (float)healthField.GetValue(mush);
        bool damageable = (bool)damageableField.GetValue(mush);

        Assert.Greater(health, 0f, "Mushroom should have positive health after Awake.");
        Assert.IsTrue(damageable, "Mushroom should be damageable after Awake.");
    }

    [TearDown]
    public void TearDown()
    {
        if (mushObj != null) Object.DestroyImmediate(mushObj);
        if (playerGO != null) Object.DestroyImmediate(playerGO);
    }


}

public class SafePlayerStub : MonoBehaviour { }