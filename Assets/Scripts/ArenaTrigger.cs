using System.Collections;
using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    #region Fields
    [Header("Arena Settings")]
    [SerializeField] private GameObject doorWall;
    [SerializeField] private EnemyLogic[] enemies;
    [SerializeField] private bool arenaActivated = false;
    #endregion
    #region Enter
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!arenaActivated && other.CompareTag("Player"))
        {
            arenaActivated = true;
            ActivateArena();
        }
    }


    private void ActivateArena()
    {
        if (doorWall != null)
            doorWall.SetActive(true); 

        StartCoroutine(CheckEnemies());
    }
    #endregion
    #region Exit
    private IEnumerator CheckEnemies()
    {
        while (!AllEnemiesDefeated())
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (doorWall != null)
            doorWall.SetActive(false);
    }

    private bool AllEnemiesDefeated()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null) return false; 
        }
        return true;
    }
    #endregion
}
