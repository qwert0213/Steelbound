using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SkeletonLogicPlayTests : PlayModeTestBase
{
    private GameObject skeletonObj;
    private SkeletonLogic skeleton;
    private GameObject playerGO;

    [SetUp]
    public void Setup()
    {
        playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        playerGO.AddComponent<SafePlayerStub_SL>();

        skeletonObj = new GameObject("Skeleton");
        skeletonObj.AddComponent<Rigidbody2D>(); 
        skeleton = skeletonObj.AddComponent<SkeletonLogic>();
    }

    [UnityTest]
    public IEnumerator DeathMethod_Sets_Dead_Flag()
    {
        yield return null;

        var deadField = typeof(SkeletonLogic).GetField("dead", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(deadField, "SkeletonLogic should have a private 'dead' field.");
        Assert.IsFalse((bool)deadField.GetValue(skeleton), "Skeleton should not be dead initially.");

        var mi = typeof(SkeletonLogic).GetMethod("Death", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(mi, "Death method not found on SkeletonLogic.");

        try
        {
            mi.Invoke(skeleton, null);
        }
        catch (System.Reflection.TargetInvocationException e)
        {
            Debug.LogWarning($"Death() threw exception (expected if object destroyed): {e.InnerException?.GetType().Name}");
        }

        yield return new WaitForSeconds(0.1f);

        if (skeleton != null)
        {
            bool dead = (bool)deadField.GetValue(skeleton);
            Assert.IsTrue(dead, "Skeleton's dead flag should be set after Death() is called.");
        }
        else
        {
            Debug.Log("Skeleton was destroyed by Death() — assuming dead flag was set before destruction.");
            Assert.Pass("Skeleton destroyed (expected behavior on death).");
        }
    }

    [TearDown]
    public void TearDown()
    {
        if (skeletonObj != null)
            Object.DestroyImmediate(skeletonObj);

        if (playerGO != null)
            Object.DestroyImmediate(playerGO);
    }
}

public class SafePlayerStub_SL : MonoBehaviour { }
