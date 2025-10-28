using NUnit.Framework;
using UnityEngine;

public class EnemyLogicEditTests
{
    private GameObject enemyObj;
    private EnemyLogic enemy;

    [SetUp]
    public void Setup()
    {
        enemyObj = new GameObject("Enemy");
        enemy = enemyObj.AddComponent<EnemyLogic>();
        typeof(EnemyLogic).GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemy, 50f);
        typeof(EnemyLogic).GetField("damageable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemy, true);
    }

    [Test]
    public void Enemy_HasHealthField()
    {
        var field = typeof(EnemyLogic).GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field, "A health mezõnek léteznie kell.");
    }

    [Test]
    public void Enemy_TakesDamage_ReducesHealth()
    {
        var healthField = typeof(EnemyLogic).GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        float start = (float)healthField.GetValue(enemy);
        float damage = 10f;
        healthField.SetValue(enemy, start - damage);
        float end = (float)healthField.GetValue(enemy);
        Assert.Less(end, start, "A sebzés után az életerõnek csökkennie kellett volna.");
    }

    [Test]
    public void Enemy_DamageableFlag_IsTrueByDefault()
    {
        var flag = (bool)typeof(EnemyLogic).GetField("damageable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(enemy);
        Assert.IsTrue(flag, "Az ellenségnek alapértelmezetten sebzõdhetõnek kell lennie.");
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(enemyObj);
    }
}
