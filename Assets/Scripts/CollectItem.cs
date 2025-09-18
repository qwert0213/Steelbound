using UnityEngine;

public class CollectItem : MonoBehaviour
{
    [SerializeField] protected int amount;
    [SerializeField] protected bool collect = false;
    [SerializeField] protected GameObject counter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            collect = true;
        }
    }
}
