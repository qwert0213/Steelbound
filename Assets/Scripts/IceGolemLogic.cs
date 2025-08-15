using System.Collections;
using UnityEngine;

public class IceGolemLogic : EnemyLogic
{
    #region Fields
    [Header("IceGolem Settings")]
    [SerializeField] private float overshootDistance = 1f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private float distToPlayer;
    private bool canDamagePlayer = true;
    private float playerDamageCooldown = 0f;
    private float playerDamageCooldownTime = 1f;
    #endregion

    #region Assignments
    private void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        startingPositionX = transform.position.x;
        positionX = startingPositionX;

        speed = 3.0f;
        visionRange = 5.0f;
        attackRange = visionRange;
        attackResetCooldown = 2.0f;
        health = 1;
        damage = 1;
        pushforce = 0.0f;
        damageable = false;
        canAttack = true;
        blockableAttack = false;
    }
    #endregion

    #region Movement
    private void FixedUpdate()
    {
        if (overrideControl) return;
    }
    #endregion

    #region Collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && health > 0)
        {
            Collider2D playerCol = collision.collider;
            float playerBottom = playerCol.bounds.min.y;
            float golemTop = boxCollider.bounds.max.y;

            if (playerBottom >= golemTop - 0.05f)
            {
                health = 0;
                canAttack = false;
                overrideControl = false;
                body.linearVelocity = Vector2.zero;
                animator.SetTrigger("Die");
                PlayDeathSound();
            }
            else if (canDamagePlayer)
            {
                player.TakeDamage(damage, pushforce);
                PlayDamageSound();
                canDamagePlayer = false;
                playerDamageCooldown = 0f;
            }
        }
    }
    #endregion

    #region Update & Direction
    private void Update()
    {
        if (overrideControl) return;

        distToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);

        if (player.transform.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
            direction = 1;
        }
        else
        {
            spriteRenderer.flipX = true;
            direction = -1;
        }

        if (distToPlayer > visionRange)
            animator.SetBool("Run", false);

        if (distToPlayer < attackRange && canAttack && player.Controllable)
        {
            canAttack = false;
            StartCoroutine(Attack());
        }

        damageCooldown += Time.deltaTime;
        if (damageCooldown > 1.0f)
        {
            damageable = false;
            animator.SetBool("Damageable", false);
        }

        playerDamageCooldown += Time.deltaTime;
        if (playerDamageCooldown > playerDamageCooldownTime)
        {
            canDamagePlayer = true;
        }
    }
    #endregion

    #region Attack
    protected override IEnumerator Attack()
    {
        overrideControl = true;
        int attackDir = (player.transform.position.x > transform.position.x) ? 1 : -1;
        float targetPos = player.transform.position.x + attackDir * overshootDistance;

        animator.SetBool("Run", true);

        while (((attackDir > 0 && transform.position.x < targetPos) || (attackDir < 0 && transform.position.x > targetPos)) && health > 0)
        {
            body.linearVelocity = new Vector2(attackDir * speed, body.linearVelocity.y);
            yield return null;
        }

        body.linearVelocity = Vector2.zero;
        animator.SetBool("Run", false);

        StartCoroutine(ResetAttack(attackResetCooldown));
        overrideControl = false;
    }
    #endregion

    protected override void PlayDeathSound() { AudioManager.Instance.PlaySFX(AudioManager.Instance.iceGolemDeath); }
    protected override void PlayDamageSound() { AudioManager.Instance.PlaySFX(AudioManager.Instance.iceGolemRun); }
}
