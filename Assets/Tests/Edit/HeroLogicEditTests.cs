using NUnit.Framework;
using UnityEngine;

public class HeroLogicEditTests
{
    private GameObject heroObj;
    private HeroLogic hero;

    [SetUp]
    public void Setup()
    {
        heroObj = new GameObject("Hero");
        hero = heroObj.AddComponent<HeroLogic>();
    }

    [Test]
    public void Hero_HasAttackFlag()
    {
        var field = typeof(HeroLogic).GetField("isAttacking", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field, "A hõsnek rendelkeznie kell isAttacking mezõvel.");
    }

    [Test]
    public void Hero_StartsNotRetreated()
    {
        var field = typeof(HeroLogic).GetField("hasRetreated", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                  ?? typeof(HeroLogic).GetField("hasRetreated", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bool value = (bool)field.GetValue(hero);
        Assert.IsFalse(value, "A hõsnek kezdetben nem szabad visszavonulva lennie.");
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(heroObj);
    }
}
