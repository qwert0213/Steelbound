using UnityEngine;

public class CollectKey : CollectItem
{
    [SerializeField] private KeyCount keyCounter;

    private void Awake()
    {
        amount = 1;
        keyCounter = GameObject.FindGameObjectWithTag("KeyScore").GetComponent<KeyCount>();
    }
    private void Update()
    {
        if (collect)
        {
            keyCounter.AddQuantity(amount);
            Destroy(gameObject);
        }
    }
}
