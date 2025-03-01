using System.Collections;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    #region Fields
    [Header("Components")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody2D body;
    [SerializeField] protected BoxCollider2D boxCollider;
    [SerializeField] protected PlayerMovement player;
    [Header("Enemy Stats")]
    [SerializeField] protected float health;
    [SerializeField] protected float visionRange;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float damage;
    [SerializeField] protected float damageCooldown;
    [SerializeField] protected float attackResetCooldown;
    [SerializeField] protected bool damageable;
    [SerializeField] protected bool canAttack;
    [Header("Movement Variables")]
    [SerializeField] protected int direction = -1;
    [SerializeField] protected float patrolDistance;
    [SerializeField] protected float speed;
    [SerializeField] protected float positionX;
    [SerializeField] protected float startingPositionX;
    [SerializeField] protected bool patrolling;
    #endregion

    #region Delayed actions
    protected IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Attack");
        StartCoroutine(ResetAttack(attackResetCooldown));
    }
    protected IEnumerator ResetAttack(float time)
    {
        yield return new WaitForSeconds(time);
        canAttack = true;
    }
    #endregion

    #region Animation Events
    public void Dead()
    {
        Destroy(gameObject);
    }
    public void TryDamagePlayer()
    {
        if (Mathf.Abs(player.transform.position.x - positionX) < attackRange)
        {
            player.TakeDamage(damage);
        }
    }
    #endregion
}

