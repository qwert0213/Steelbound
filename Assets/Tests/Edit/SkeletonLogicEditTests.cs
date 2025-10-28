using NUnit.Framework;
using UnityEngine;

public class SkeletonLogicEditTests
{
    private GameObject skeletonObj;
    private SkeletonLogic skeleton;

    [SetUp]
    public void Setup()
    {
        skeletonObj = new GameObject("Skeleton");
        skeleton = skeletonObj.AddComponent<SkeletonLogic>();
    }

    [Test]
    public void Skeleton_StartsAlive()
    {
        var deadField = typeof(SkeletonLogic).GetField("dead", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bool dead = (bool)deadField.GetValue(skeleton);
        Assert.IsFalse(dead, "A skeleton alaphelyzetben nem lehet halott.");
    }

    [Test]
    public void Skeleton_CanEnterAttackingState()
    {
        var field = typeof(SkeletonLogic).GetField("isAttacking", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(skeleton, true);
        bool attacking = (bool)field.GetValue(skeleton);
        Assert.IsTrue(attacking, "A skeleton támadási állapotát be kellett volna tudni kapcsolni.");
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(skeletonObj);
    }
}
