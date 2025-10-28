using NUnit.Framework;
using UnityEngine;

public class MushroomLogicEditTests
{
    private GameObject mushObj;
    private MushroomLogic mush;

    [SetUp]
    public void Setup()
    {
        mushObj = new GameObject("Mushroom");
        mush = mushObj.AddComponent<MushroomLogic>();
    }

    [Test]
    public void Mushroom_HasPatrolDistanceField()
    {
        var field = typeof(MushroomLogic).GetField("patrolDistance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(field, "A patrolDistance mezõnek léteznie kell.");
    }

    [Test]
    public void Mushroom_Speed_IsPositive()
    {
        var speedField = typeof(MushroomLogic).GetField("speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        float speed = (float)speedField.GetValue(mush);
        Assert.GreaterOrEqual(speed, 0f, "A mozgási sebességnek pozitívnak kell lennie.");
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(mushObj);
    }
}
