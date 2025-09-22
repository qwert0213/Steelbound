using UnityEngine;

public class WolfActivator : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject[] wolves;
    #endregion
    #region Activate
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
    #endregion
}
