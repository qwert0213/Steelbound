using NUnit.Framework;
using UnityEngine;

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
}
