using System.Collections;
using UnityEngine;

public class WolfLogic : EnemyLogic
{
    #region Fields
    [Header("Wolf Settings")]
    [SerializeField] private float leapForceX = 6f;
    [SerializeField] private float leapForceY = 3f;
    [SerializeField] private float retreatSpeed = 1.5f;
    [SerializeField] private float retreatTime = 1.0f;
    [SerializeField] private float maxVerticalDistance = 1.0f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject keyPrefab; 
    private float distToPlayer;
    private bool isAttacking;
    private Vector3 deathPosition;
    #endregion
    #region Assignements
    private void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        startingPositionX = transform.position.x;
        positionX = startingPositionX;

        speed = 2.0f;
        attackRange = 2.05f;
        attackResetCooldown = 1.2f;
        health = 2;
        damage = 1;
        pushforce = 5.0f;
        damageable = true;
        canAttack = true;
        blockableAttack = true;
        isAttacking = false;
    }
    #endregion
    #region Movement
    private void FixedUpdate()
    {
        if (overrideControl) return;
        CheckDamage();
        if (canAttack)
        {
            int moveDir = player.transform.position.x > transform.position.x ? 1 : -1;
            body.linearVelocity = new Vector2(moveDir * speed, body.linearVelocity.y);
            animator.SetBool("Patrolling", true);
        }
        else
        {
            body.linearVelocity = new Vector2(-direction * retreatSpeed, body.linearVelocity.y);
            animator.SetBool("Patrolling", false);
        }
    }
    #endregion
    #region Damage Taking

    private void CheckDamage()
    {
        distToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (damageable && player.IsAttacking && distToPlayer < player.AttackRange+0.3f)
        {
            if ((player.Direction == 1 && player.transform.position.x - positionX < 0) ||
                (player.Direction == -1 && player.transform.position.x - positionX > 0))
            {
                health -= player.AttackDamage;
                if (health <= 0)
                {
                    damageable = false;
                    isAttacking = false;
                    overrideControl = false;
                    canAttack = false;
                    body.linearVelocity = Vector2.zero;
                    deathPosition = transform.position;
                    WolfLogic[] wolves = FindObjectsByType<WolfLogic>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                    if (wolves.Length == 1 && keyPrefab != null)
                    {
                        Instantiate(keyPrefab, deathPosition, Quaternion.identity);
                    }
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
    }
    #endregion
    #region Direction
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
        if (distToPlayer <= attackRange && canAttack && player.Controllable)
        {
            canAttack = false;
            StartCoroutine(Attack());
        }
    }
    #endregion
    #region Attack
    protected override IEnumerator Attack()
    {
        isAttacking = true;
        overrideControl = true;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.3f);
        if (!isAttacking) yield break;
        int leapDir = (player.transform.position.x > transform.position.x) ? 1 : -1;
        body.linearVelocity = Vector2.zero;
        body.AddForce(new Vector2(leapDir * leapForceX, leapForceY), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.18f);
        if (!isAttacking) yield break;
        Vector2 dirToEnemy = (transform.position - player.transform.position).normalized;
        bool facingEnemy = (player.Direction == 1 && dirToEnemy.x > 0) || (player.Direction == -1 && dirToEnemy.x < 0);
        bool canBlock = blockableAttack && player.Blocking && facingEnemy;
        float distX = Mathf.Abs(player.transform.position.x - transform.position.x);
        float distY = Mathf.Abs(player.transform.position.y - transform.position.y);

        if (distX < attackRange && distY <= maxVerticalDistance && !canBlock)
        {
            player.TakeDamage(damage, pushforce);
            PlayDamageSound();
        }

        StartCoroutine(ResetAttack(attackResetCooldown));
        int retreatDir = -leapDir;
        float retreatTimer = 0f;
        while (retreatTimer < retreatTime && !canAttack && isAttacking)
        {
            body.linearVelocity = new Vector2(retreatDir * retreatSpeed, body.linearVelocity.y);
            retreatTimer += Time.deltaTime;
            yield return null;
        }
        overrideControl = false;
        isAttacking = false;
    }
    #endregion


    protected override void PlayDeathSound() { AudioManager.Instance.PlaySFX(AudioManager.Instance.wolfDeath); }
    protected override void PlayHurtSound() { AudioManager.Instance.PlaySFX(AudioManager.Instance.wolfHurt); }
    protected override void PlayDamageSound() { AudioManager.Instance.PlaySFX(AudioManager.Instance.wolfAttack); }
}