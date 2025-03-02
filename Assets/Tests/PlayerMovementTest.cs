using NUnit.Framework;
using UnityEngine;
using Player;
[TestFixture]
public class PlayerMovementTests
{
    private GameObject player;
    private PlayerMovement playerMovement;
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;

    [SetUp]
    public void Setup()
    {
        player = new GameObject("Player");
        player.AddComponent<Rigidbody2D>();
        player.AddComponent<BoxCollider2D>();
        playerMovement = player.AddComponent<PlayerMovement>();
        animator = player.AddComponent<Animator>();
        boxCollider = player.GetComponent<BoxCollider2D>();
        playerMovement.GetType().GetField("animator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(playerMovement, animator);

        string controllerPath = "Assets/Animations/Player/Player.controller";
        RuntimeAnimatorController animatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
        if (animatorController != null)
        {
            animator.runtimeAnimatorController = animatorController;
        }
        else
        {
            Debug.LogError("Animator Controller could not be loaded from path: " + controllerPath);
        }
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(player);
    }

    [Test]
    public void TestComponentAssignment()
    {
        Assert.IsNotNull(player.GetComponent<Rigidbody2D>(), "Player should have Rigidbody2D.");
        Assert.IsNotNull(player.GetComponent<Animator>(), "Player should have Animator.");
        Assert.IsNotNull(player.GetComponent<BoxCollider2D>(), "Player should have BoxCollider2D.");
    }

    [Test]
    public void TestAnimatorControllerAssignment()
    {
        Assert.AreEqual(animator.runtimeAnimatorController.name, "Player", "Animator Controller should be 'Player'.");
    }

    [Test]
    public void TestDefaultValues()
    {
        Assert.AreEqual(1, playerMovement.Direction);
        Assert.AreEqual(5.0f, playerMovement.Speed);
        Assert.AreEqual(3.0f, playerMovement.JumpForce);
        Assert.AreEqual(4.0f, playerMovement.RollForce);
    }

    [Test]
    public void TestTakingDamage()
    {
        float initialHealth = playerMovement.GetType().GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(playerMovement) as float? ?? 0f;
        playerMovement.TakeDamage(1.0f);
        float newHealth = playerMovement.GetType().GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(playerMovement) as float? ?? 0f;
        Assert.AreEqual(initialHealth - 1.0f, newHealth);
    }

    [Test]
    public void TestDeath()
    {
        playerMovement.TakeDamage(playerMovement.GetType().GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(playerMovement) as float? ?? 0f);
        bool controllable = (bool)playerMovement.GetType().GetField("controllable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(playerMovement);
        Assert.IsFalse(controllable);
    }
}
