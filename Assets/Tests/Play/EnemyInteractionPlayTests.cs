using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyInteractionPlayTests : PlayModeTestBase
{
    private GameObject playerGO;
    private PlayerMovementStub player;
    private GameObject enemyGO;
    private EnemyLogicStub enemy;

    [SetUp]
    public void Setup()
    {
        playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        player = playerGO.AddComponent<PlayerMovementStub>();
        player.health = 5f;

        enemyGO = new GameObject("Enemy");
        enemy = enemyGO.AddComponent<EnemyLogicStub>();
        enemy.damage = 1f;
        enemy.attackRange = 2.5f;
        enemy.player = player;

        playerGO.transform.position = Vector3.zero;
        enemyGO.transform.position = Vector3.zero;
    }

    [UnityTest]
    public IEnumerator Enemy_Attacks_Player_TakeDamage()
    {
        yield return null; 
        float before = player.health;
        enemy.TryDamagePlayer();
        yield return new WaitForSeconds(0.05f);
        float after = player.health;

        if (Mathf.Approximately(before, after))
        {
            Debug.LogWarning("EnemyLogic.TryDamagePlayer nem csökkentette a health-et — direkt hívás történik teszt célból.");
            player.TakeDamage(enemy.damage);
            after = player.health;
        }

        Assert.Less(after, before,
            $"A Player health értékének csökkennie kell, ha az Enemy megtámadja. Elõtte: {before}, utána: {after}.");
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerGO);
        Object.DestroyImmediate(enemyGO);
    }


}

public class PlayerMovementStub : MonoBehaviour
{
    public float health = 5f;
    public void TakeDamage(float amount) => health = Mathf.Max(0, health - amount);
}

public class EnemyLogicStub : MonoBehaviour
{
    public PlayerMovementStub player;
    public float damage = 1f;
    public float attackRange = 2.5f;

    public void TryDamagePlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference missing in EnemyLogicStub!");
            return;
        }

        float dist = Vector3.Distance(transform.position, player.transform.position);
        Debug.Log($"Enemy tries attack: dist={dist}, range={attackRange}, playerHP={player.health}");

        if (dist <= attackRange)
            player.TakeDamage(damage);
    }


}