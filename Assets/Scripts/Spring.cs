using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour
{
    #region Fields
    public float bounceForce = 5f;
    public float cooldown = 2f;
    private Animator animator;
    private bool isOnCooldown = false;
    #endregion
    #region Component Retrieval
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    #endregion
    #region Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOnCooldown)
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
            }

            if (animator != null)
            {
                animator.SetTrigger("Activate");
            }

            StartCoroutine(StartCooldown());
        }
    }
    #endregion
    #region Cooldown
    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
    #endregion
}
