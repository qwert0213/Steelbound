using UnityEngine;

public class CollectCoin : CollectItem
{
    [SerializeField] private CoinCount coinCounter;

    private void Awake()
    {
        amount = 10;
        coinCounter = GameObject.FindGameObjectWithTag("CoinScore").GetComponent<CoinCount>();
    }
    private void Update()
    {
        if (collect)
        {
            coinCounter.AddQuantity(amount);
            Destroy(gameObject);
        }
    }
}
