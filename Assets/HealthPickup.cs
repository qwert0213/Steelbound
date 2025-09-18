using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            if (player.Health < player.MaxHealth)
            {
                player.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
