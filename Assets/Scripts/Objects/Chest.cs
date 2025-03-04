using Player;
using UnityEngine;

public class Chest : MonoBehaviour
{
    #region Fields
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform chestPosition;
    [SerializeField] private BoxCollider2D self;
    [SerializeField] private bool opened = false;
    [SerializeField] private GameObject coinPrefab;
    #endregion

    private void Awake()
    {
        #region Component Retrieval
        animator = GetComponent<Animator>();
        chestPosition = GetComponent<Transform>();
        self = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        #endregion
    }

    private void Update()
    {
        #region TryOpening
        if (!opened && player.Interacting && Mathf.Abs(player.transform.position.x - chestPosition.position.x) < player.AttackRange && Mathf.Abs(player.transform.position.y - chestPosition.position.y) < player.AttackRange)
        {
            opened = true;
            animator.SetTrigger("Open");
            self.enabled = false;
        }
        #endregion
    }
    #region Animation Events
    private void SpawnCoins() {
        for (int i = 0; i < 3; i++)
        {
            GameObject coin = Instantiate(coinPrefab, chestPosition.position, Quaternion.identity);
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
            Vector2 forceDirection = new Vector2(Random.Range(-1f, 1f), 3f);
            rb.AddForce(forceDirection, ForceMode2D.Impulse);
        }
    }
    #endregion
}
