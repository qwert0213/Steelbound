using UnityEngine;


public class CollectItem : MonoBehaviour
{
    [SerializeField] protected int amount;
    [SerializeField] protected bool collect = false;
    [SerializeField] protected GameObject counter;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) collect = true;
    }
}
