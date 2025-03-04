using UnityEngine;

public class CollectItem : MonoBehaviour
{
    [SerializeField] private int score = 1;
    [SerializeField] private CoinCount coinCounter;

    private void Awake()
    {
        coinCounter = GameObject.FindGameObjectWithTag("Score").GetComponent<CoinCount>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            coinCounter.AddScore(score);
            Destroy(this.gameObject);
        }
    }
}
