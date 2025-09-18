using UnityEngine;

public class WolfActivator : MonoBehaviour
{
    [SerializeField] private GameObject[] wolves;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            foreach (GameObject wolf in wolves)
            {
                wolf.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}
