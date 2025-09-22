using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    #region Fields
    [SerializeField] private int healAmount = 1;
    #endregion
    #region Pickup
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
    #endregion
}
