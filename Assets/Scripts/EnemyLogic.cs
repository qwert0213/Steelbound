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
    [SerializeField] protected float pushforce;
    [SerializeField] protected float damageCooldown;
    [SerializeField] protected float attackResetCooldown;
    [SerializeField] protected bool damageable;
    [SerializeField] protected bool canAttack;
    [SerializeField] protected bool blockableAttack;
    [Header("Movement Variables")]
    [SerializeField] protected int direction = -1;
    [SerializeField] protected float patrolDistance;
    [SerializeField] protected float speed;
    [SerializeField] protected float positionX;
    [SerializeField] protected float startingPositionX;
    [SerializeField] protected bool patrolling;
    protected bool overrideControl = false;
    float enemyPlayerDistance;
    #endregion
    void Update()
    {
        if (overrideControl) return;

        positionX = transform.position.x;
        enemyPlayerDistance = Mathf.Abs(player.transform.position.x - positionX);
        #region Direction
        if (positionX > startingPositionX + patrolDistance)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            direction = -1;
        }
        else if (positionX < startingPositionX - patrolDistance)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            direction = 1;
        }
        #endregion

        #region Patrol
        if (enemyPlayerDistance > visionRange)
        {
            body.linearVelocity = new Vector2(direction * speed, body.linearVelocity.y);
            animator.SetBool("Patrolling", true);
        }
        else
        {
            if (player.Direction == 1) direction = -1;
            else direction = -1;
            animator.SetBool("Patrolling", false);
        }
        #endregion

        #region Damage Taking
        if (damageable && player.IsAttacking && enemyPlayerDistance < player.AttackRange)
        {
            if ((player.Direction == 1 && player.transform.position.x - positionX < 0) || (player.Direction == -1 && player.transform.position.x - positionX > 0))
            {
                health -= player.AttackDamage;
                if (health <= 0)
                {
                    damageable = false;
                    animator.SetTrigger("Die");
                }
                else
                {
                    damageable = false;
                    animator.SetBool("Damageable", false);
                    damageCooldown = 0;
                    PlayHurtSound();
                }
            }
        }
        damageCooldown += Time.deltaTime;
        if (damageCooldown > 1.0f)
        {
            damageable = true;
            animator.SetBool("Damageable", true);
        }
        #endregion

        #region Attack Start
        if (enemyPlayerDistance < attackRange && canAttack && player.Controllable)
        {
            canAttack = false;
            direction = -1 * player.Direction;
            StartCoroutine(Attack());
        }
        #endregion

    }
    #region Virtual Methods
    protected virtual void PlayDeathSound()
    {
    }
    protected virtual void PlayHurtSound()
    {
    }
    protected virtual void PlayDamageSound()
    {
    }
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
        if (Application.IsPlaying(gameObject))
        {
            PlayDeathSound();
            Destroy(gameObject); 
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    public void TryDamagePlayer()
    {
        Vector2 dirToEnemy = (transform.position - player.transform.position).normalized;
        bool facingEnemy = (player.Direction == 1 && dirToEnemy.x > 0) || (player.Direction == -1 && dirToEnemy.x < 0);

        bool canBlock = blockableAttack && player.Blocking && facingEnemy;

        if (enemyPlayerDistance < attackRange && !canBlock)
        {
            player.TakeDamage(damage, pushforce);
            PlayDamageSound();
        }
    }
    #endregion
}

