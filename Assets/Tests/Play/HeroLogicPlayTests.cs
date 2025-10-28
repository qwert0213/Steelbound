using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HeroLogicPlayTests : PlayModeTestBase
{
    private GameObject heroObj;
    private HeroLogic hero;
    private GameObject playerGO;

    [SetUp]
    public void Setup()
    {
        playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        playerGO.AddComponent<SafePlayerMovementStub>();

        heroObj = new GameObject("Hero");
        hero = heroObj.AddComponent<HeroLogic>();
    }

    [UnityTest]
    public IEnumerator Awake_Sets_Default_Stats_On_Hero()
    {
        yield return null;

        var healthField = typeof(EnemyLogic).GetField("health", BindingFlags.NonPublic | BindingFlags.Instance);
        var attackRangeField = typeof(EnemyLogic).GetField("attackRange", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(healthField, "EnemyLogic.health field not found");
        Assert.IsNotNull(attackRangeField, "EnemyLogic.attackRange field not found");

        float health = (float)healthField.GetValue(hero);
        float attackRange = (float)attackRangeField.GetValue(hero);

        Assert.AreEqual(3f, health, 0.001f, "Hero default health expected to be 3.");
        Assert.AreEqual(2.05f, attackRange, 0.01f, "Hero default attackRange expected to be ~2.05.");
    }

    [TearDown]
    public void TearDown()
    {
        if (heroObj != null) Object.DestroyImmediate(heroObj);
        if (playerGO != null) Object.DestroyImmediate(playerGO);
    }


}

public class SafePlayerMovementStub : MonoBehaviour
{
    public float health = 5f;
    public void TakeDamage(float amount) => health -= amount;
}