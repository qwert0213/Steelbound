using UnityEngine;

public class EnemyActivatorTrigger : MonoBehaviour
{
    #region Fields
    [Header("Enemies to Activate")]
    [SerializeField] private GameObject[] enemiesToActivate;
    [SerializeField] private bool activated = false;
    #endregion

    #region Component Retrieval
    private void Start()
    {
        foreach (var enemy in enemiesToActivate)
        {
            if (enemy != null)
                enemy.SetActive(false);
        }
    }
    #endregion
    #region Activate
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activated && other.CompareTag("Player"))
        {
            activated = true;
            foreach (var enemy in enemiesToActivate)
            {
                if (enemy != null)
                    enemy.SetActive(true);
            }
        }
    }
    #endregion
}
