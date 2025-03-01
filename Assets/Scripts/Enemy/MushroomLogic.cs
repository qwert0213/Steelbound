using UnityEngine;

public class MushroomLogic : EnemyLogic
{
    private void Awake()
    {
        #region Component Retrieval
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        #endregion

        #region Base Variable Settings
        positionX = GetComponent<Transform>().position.x;
        startingPositionX = GetComponent<Transform>().position.x;
        patrolDistance = 1.5f;
        speed = 2.0f;
        attackRange = 2.05f;
        visionRange = 5.0f;
        attackResetCooldown = 1.2f;
        health = 2;
        damage = 1;
        patrolling = true;
        damageable = true;
        canAttack = true;
        #endregion

    }
    void Update()
    {
            #region Direction
            positionX = transform.position.x;
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
            if (Mathf.Abs(player.transform.position.x - positionX) > visionRange)
            {
                body.linearVelocity = new Vector2(direction * speed, body.linearVelocity.y);
                animator.SetBool("Patrolling", true);
            }
            else animator.SetBool("Patrolling", false);
            #endregion

            #region Damage Taking
            if (damageable && player.IsAttacking && Mathf.Abs(player.transform.position.x - positionX) < player.AttackRange)
            {
                if ((player.Direction == 1 && player.transform.position.x - positionX < 0) || (player.Direction == -1 && player.transform.position.x - positionX > 0))
                {
                    health -= player.AttackDamage;
                    if (health <= 0)
                    {
                        animator.SetTrigger("Die");
                    }
                    else
                    {
                        damageable = false;
                        animator.SetBool("Damageable", false);
                        damageCooldown = 0;
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
            if (Mathf.Abs(player.transform.position.x - positionX) < attackRange && canAttack && player.Controllable)
            {
                canAttack = false;
                direction = -1 * player.Direction;
                StartCoroutine(Attack());
            }
            #endregion
        
    }
}
