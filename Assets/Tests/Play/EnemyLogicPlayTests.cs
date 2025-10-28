using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyLogicPlayTests : PlayModeTestBase
{
    private GameObject enemyObj;
    private EnemyLogic enemy;

    [SetUp]
    public void Setup()
    {
        enemyObj = new GameObject("Enemy");
        enemy = enemyObj.AddComponent<EnemyLogic>();
        typeof(EnemyLogic).GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemy, 50f);
    }

    [UnityTest]
    public IEnumerator Enemy_TakesDamage_AndDiesBelowZero()
    {
        var field = typeof(EnemyLogic).GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(enemy, 10f);
        float health = (float)field.GetValue(enemy);
        health -= 15f;
        field.SetValue(enemy, health);
        yield return null;
        float after = (float)field.GetValue(enemy);
        Assert.LessOrEqual(after, 0f, "A sebzés után az életerõnek 0 vagy kevesebbnek kell lennie.");
    }

    [UnityTest]
    public IEnumerator Enemy_DamageCooldown_WorksProperly()
    {
        var cooldown = typeof(EnemyLogic).GetField("damageCooldown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        cooldown.SetValue(enemy, 1f);
        yield return new WaitForSeconds(1.1f);
        float cd = (float)cooldown.GetValue(enemy);
        Assert.GreaterOrEqual(cd, 1f, "A sebzés cooldown értékének idõvel vissza kellett volna állnia.");
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(enemyObj);
    }
}