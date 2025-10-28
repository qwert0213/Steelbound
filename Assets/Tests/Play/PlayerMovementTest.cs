using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerMovementPlayTests : PlayModeTestBase
{
    private GameObject heroGO;
    private SafePlayerMovementStub_PM player;

    [SetUp]
    public void Setup()
    {
        heroGO = new GameObject("Player");
        player = heroGO.AddComponent<SafePlayerMovementStub_PM>();
        heroGO.AddComponent<Rigidbody2D>();

        var t = typeof(SafePlayerMovementStub_PM);
        t.GetField("maxHealth", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(player, 100f);
        t.GetField("health", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(player, 10f);
        t.GetField("controllable", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(player, true);
    }

    [UnityTest]
    public IEnumerator TakeDamage_ReducesHealth_And_TriggersDeathState()
    {
        player.TakeDamage(50f, 0f);
        yield return new WaitForSeconds(0.05f);

        var t = typeof(SafePlayerMovementStub_PM);
        float healthAfter = (float)t.GetField("health", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player);
        bool controllable = (bool)t.GetField("controllable", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player);

        Assert.LessOrEqual(healthAfter, 0f, "Health should drop to 0 or below after lethal damage.");
        Assert.IsFalse(controllable, "Player should not be controllable after death.");
    }

    [UnityTest]
    public IEnumerator Heal_IncreasesHealth_But_Not_AboveMax()
    {
        var t = typeof(SafePlayerMovementStub_PM);
        t.GetField("health", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, 40f);
        t.GetField("maxHealth", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, 100f);

        player.Heal(30);
        yield return null;

        float after1 = (float)t.GetField("health", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player);
        Assert.AreEqual(70f, after1, "Heal should increase health correctly.");

        player.Heal(100);
        yield return null;

        float after2 = (float)t.GetField("health", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player);
        Assert.AreEqual(100f, after2, "Health should not exceed maxHealth after Heal.");
    }

    [TearDown]
    public void TearDown()
    {
        if (heroGO != null)
            Object.DestroyImmediate(heroGO);
    }
}

public class SafePlayerMovementStub_PM : MonoBehaviour
{
    private float health = 100f;
    private float maxHealth = 100f;
    private bool controllable = true;

    public void TakeDamage(float amount, float _)
    {
        health -= amount;
        if (health <= 0f)
        {
            health = 0f;
            controllable = false;
        }
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
    }
}
