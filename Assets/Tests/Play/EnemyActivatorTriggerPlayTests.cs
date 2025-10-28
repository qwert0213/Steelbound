using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyActivatorTriggerPlayTests : PlayModeTestBase
{
    private GameObject triggerGO;
    private EnemyActivatorTrigger trigger;
    private GameObject enemyGO;
    private GameObject playerGO;

    [SetUp]
    public void Setup()
    {
        triggerGO = new GameObject("ActivatorTrigger");
        var collider = triggerGO.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        trigger = triggerGO.AddComponent<EnemyActivatorTrigger>();

        enemyGO = new GameObject("EnemyToActivate");
        enemyGO.hideFlags = HideFlags.DontSave;
        enemyGO.SetActive(false);

        var field = typeof(EnemyActivatorTrigger)
            .GetField("enemiesToActivate", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(trigger, new GameObject[] { enemyGO });

        playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        playerGO.hideFlags = HideFlags.DontSave;
        playerGO.AddComponent<BoxCollider2D>();
    }

    [UnityTest]
    public IEnumerator TriggerActivates_Enemies_OnPlayerEnter()
    {
        var mi = typeof(EnemyActivatorTrigger)
            .GetMethod("OnTriggerEnter2D", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(mi, "Nem található az OnTriggerEnter2D metódus az EnemyActivatorTrigger-ben.");

        var collider = playerGO.GetComponent<Collider2D>();
        mi.Invoke(trigger, new object[] { collider });

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(enemyGO != null && enemyGO.activeSelf,
            "EnemyActivatorTrigger nem aktiválta az ellenséget amikor a Player belépett a triggerbe.");

        LogAssert.NoUnexpectedReceived();
    }

    [TearDown]
    public void TearDown()
    {
        if (triggerGO != null && triggerGO.scene.IsValid()) Object.DestroyImmediate(triggerGO);
        if (enemyGO != null && enemyGO.scene.IsValid()) Object.DestroyImmediate(enemyGO);
        if (playerGO != null && playerGO.scene.IsValid()) Object.DestroyImmediate(playerGO);
    }


}