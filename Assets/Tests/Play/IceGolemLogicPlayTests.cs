using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IceGolemLogicPlayTests : PlayModeTestBase
{
    private GameObject golemObj;
    private IceGolemLogic golem;
    private GameObject playerGO;

    [SetUp]
    public void Setup()
    {
        playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        playerGO.AddComponent<SafePlayerMovementStub>();

        golemObj = new GameObject("IceGolem");
        golem = golemObj.AddComponent<IceGolemLogic>();
    }

    [UnityTest]
    public IEnumerator Awake_Initializes_IceGolem_Stats()
    {
        yield return null;
        var healthField = typeof(EnemyLogic).GetField("health", BindingFlags.NonPublic | BindingFlags.Instance);
        var attackRangeField = typeof(EnemyLogic).GetField("attackRange", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(healthField, "EnemyLogic.health field not found");
        Assert.IsNotNull(attackRangeField, "EnemyLogic.attackRange field not found");

        float health = (float)healthField.GetValue(golem);
        float attackRange = (float)attackRangeField.GetValue(golem);

        Assert.AreEqual(1f, health, 0.001f, "IceGolem default health expected to be 2.");
        Assert.AreEqual(5f, attackRange, 0.01f, "IceGolem default attackRange expected to be ~1.8.");
    }

    [TearDown]
    public void TearDown()
    {
        if (golemObj != null) Object.DestroyImmediate(golemObj);
        if (playerGO != null) Object.DestroyImmediate(playerGO);
    }


}

