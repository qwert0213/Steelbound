using System.Collections;
using UnityEngine;

public class SkeletonLogic : EnemyLogic
{
    #region Fields
    [Header("Skeleton Settings")]
    [SerializeField] private float followSpeed = 2.5f;
    [SerializeField] private GameObject trapPrefab;

    private SpriteRenderer spriteRenderer;
    private bool isAttacking = false;
    private Vector3 deathPosition;
    private bool dead = false;
    #endregion
    #region Component Retrieval
    private void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        startingPositionX = transform.position.x;
        positionX = startingPositionX;

        speed = 2.0f;
        followSpeed = 2.5f;
        attackRange = 1.8f;
        visionRange = 5.0f;
        attackResetCooldown = 1.2f;
        health = 2;
        damage = 1;
        pushforce = 3.0f;
        damageable = true;
        canAttack = true;
        blockableAttack = true;
        isAttacking = false;
    }
    #endregion
    private void Update()
    {
        #region Base
        if (!dead)
        {
            if (overrideControl) return;
            CheckDamage();

            float distToPlayerX = Mathf.Abs(player.transform.position.x - transform.position.x);
            float distToPlayerY = Mathf.Abs(player.transform.position.y - transform.position.y);

            spriteRenderer.flipX = player.transform.position.x < transform.position.x;

            if (!isAttacking && canAttack)
            {
                if (distToPlayerY <= 2f)
                {
                    if (distToPlayerX <= attackRange)
                    {
                        StartCoroutine(Attack());
                    }
                    else if (distToPlayerX <= visionRange)
                    {
                        FollowPlayer();
                    }
                    else
                    {
                        animator.SetBool("Run", false);
                        body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
                    }
                }
                else
                {
                    animator.SetBool("Run", false);
                    body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
                }
            }
        }
        #endregion
    }

    #region Run
    private void FollowPlayer()
    {
        float dir = (player.transform.position.x > transform.position.x) ? 1f : -1f;
        Vector2 targetVelocity = new Vector2(dir * followSpeed, body.linearVelocity.y);
        body.linearVelocity = Vector2.Lerp(body.linearVelocity, targetVelocity, Time.deltaTime * 5f);
        animator.SetBool("Run", true);
    }
    #endregion
    #region Attack
    protected override IEnumerator Attack()
    {
        isAttacking = true;
        overrideControl = true;
        animator.SetTrigger("Attack");
        body.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.3f);

        float distX = Mathf.Abs(player.transform.position.x - transform.position.x);
        float distY = Mathf.Abs(player.transform.position.y - transform.position.y);

        if (distX < attackRange && distY <= 2f)
        {
            Vector2 dirToEnemy = (transform.position - player.transform.position).normalized;
            bool facingEnemy = (player.Direction == 1 && dirToEnemy.x > 0) || (player.Direction == -1 && dirToEnemy.x < 0);
            bool canBlock = blockableAttack && player.Blocking && facingEnemy;

            if (!canBlock)
            {
                player.TakeDamage(damage, pushforce);
                PlayDamageSound();
            }
        }
        canAttack = false;
        StartCoroutine(ResetAttack(attackResetCooldown));

        yield return new WaitForSeconds(0.4f);

        overrideControl = false;
        isAttacking = false;
    }
    #endregion
    #region Take Damage
    private void CheckDamage()
    {
        float distToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (damageable && player.IsAttacking && distToPlayer < player.AttackRange + 0.3f)
        {
            if ((player.Direction == 1 && player.transform.position.x < transform.position.x) ||
                (player.Direction == -1 && player.transform.position.x > transform.position.x))
            {
                health -= player.AttackDamage;
                damageable = false;
                damageCooldown = 0;
                PlayHurtSound();

                if (health <= 0)
                {
                    canAttack = false;
                    Death();
                }
            }
        }
        damageCooldown += Time.deltaTime;
        if (damageCooldown > 1.0f)
        {
            damageable = true;
        }
    }
    #endregion
    #region Die
    private void Death()
    {
        dead = true;
        damageable = false;
        isAttacking = false;
        overrideControl = false;
        canAttack = false;
        body.linearVelocity = Vector2.zero;
        deathPosition = transform.position;
        animator.SetBool("Run", false);
        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;
        body.bodyType = RigidbodyType2D.Kinematic;
        animator.SetTrigger("Die");
        StartCoroutine(HandleDeath());
    }
    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(2f);

        Vector3 spawnPos = deathPosition + new Vector3(0f, -0.55f, 0f);
        if (trapPrefab != null) Instantiate(trapPrefab, spawnPos, Quaternion.identity);

        Destroy(gameObject);
    }
    #endregion
    #region Sound
    protected override void PlayDeathSound() { AudioManager.Instance.PlaySFX(AudioManager.Instance.skeletonDeath); }
    protected override void PlayHurtSound() { AudioManager.Instance.PlaySFX(AudioManager.Instance.skeletonHurt); }
    protected override void PlayDamageSound() { AudioManager.Instance.PlaySFX(AudioManager.Instance.skeletonAttack); }
    #endregion
}
