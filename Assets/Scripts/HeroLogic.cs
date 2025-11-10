using System.Collections;
using UnityEngine;

public class HeroLogic : EnemyLogic
{
    [Header("Hero Specific")]
    [SerializeField] private Transform retreatPoint;
    [SerializeField] private float retreatSpeed = 6f;
    [SerializeField] private float retreatRunSpeed = 10f;
    [SerializeField] private float retreatTime = 1.5f;
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private GameObject gnomePrefab;
    private SpriteRenderer spriteRenderer;
    private bool isAttacking = false;
    private bool isAlive = true;
    public bool hasRetreated = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        positionX = transform.position.x;
        startingPositionX = transform.position.x;
        patrolDistance = 1.5f;
        speed = 2.0f;
        attackRange = 2.05f;
        visionRange = 5.0f;
        attackResetCooldown = 1.2f;
        health = 3;
        damage = 1;
        pushforce = 5.0f;
        patrolling = true;
        damageable = true;
        canAttack = true;
        blockableAttack = true;
        canPatrol = true;
    }

    private void Update()
    {
        if (!overrideControl && !isAttacking)
        {
            float distToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);
            if (distToPlayer < visionRange && distToPlayer > attackRange && isAlive)
            {
                FollowPlayer();
            }
            else if (distToPlayer <= attackRange && canAttack && !hasRetreated)
            {
                StartCoroutine(Attack());
            }
        }
        if (!overrideControl)
            spriteRenderer.flipX = player.transform.position.x < transform.position.x;
        CheckDamage();
    }

    private void FollowPlayer()
    {
        float dir = (player.transform.position.x > transform.position.x) ? 1f : -1f;
        Vector2 targetVelocity = new Vector2(dir * followSpeed, body.linearVelocity.y);
        body.linearVelocity = Vector2.Lerp(body.linearVelocity, targetVelocity, Time.deltaTime * 5f);
        animator.SetBool("Run", true);
    }

    protected override IEnumerator Attack()
    {
        isAttacking = true;
        overrideControl = true;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.3f);
        Vector2 dirToEnemy = (transform.position - player.transform.position).normalized;
        bool facingEnemy = (player.Direction == 1 && dirToEnemy.x > 0) || (player.Direction == -1 && dirToEnemy.x < 0);
        bool canBlock = blockableAttack && player.Blocking && facingEnemy;
        float distX = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (distX < attackRange && !canBlock)
        {
            player.TakeDamage(damage, pushforce);
            PlayDamageSound();
        }
        canAttack = false;
        damageable = false;
        float retreatDir = (player.transform.position.x > transform.position.x) ? -1f : 1f;
        float retreatTimer = 0f;
        while (retreatTimer < retreatTime)
        {
            body.linearVelocity = new Vector2(retreatDir * retreatSpeed, body.linearVelocity.y);
            retreatTimer += Time.deltaTime;
            yield return null;
        }
        body.linearVelocity = Vector2.zero;
        animator.SetTrigger("Idle");
        damageable = true;
        float idleAfterRetreat = 0.6f;
        yield return new WaitForSeconds(idleAfterRetreat);
        yield return new WaitForSeconds(attackResetCooldown);
        canAttack = true;
        overrideControl = false;
        isAttacking = false;
    }

    private void CheckDamage()
    {
        float distToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (damageable && player.IsAttacking && distToPlayer < player.AttackRange + 0.3f)
        {
            if ((player.Direction == 1 && player.transform.position.x < transform.position.x) || (player.Direction == -1 && player.transform.position.x > transform.position.x))
            {
                health -= player.AttackDamage;
                damageable = false;
                animator.SetBool("Damageable", false);
                animator.SetTrigger("Hit");
                damageCooldown = 0;
                PlayHurtSound();
                if (health<= 0 && !hasRetreated)
                {
                    hasRetreated = true;
                    StartCoroutine(FinalRetreat());
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

    private IEnumerator FinalRetreat()
    {
        isAlive = false;
        overrideControl = true;
        canAttack = false;
        damageable = false;
        patrolling = false;

        if (boxCollider != null) boxCollider.enabled = false;
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
            body.bodyType = RigidbodyType2D.Kinematic;
        }

        animator.SetTrigger("Run");

        float timer = 0f;
        while (timer < 2f)
        {
            if (retreatPoint != null)
            {
                float dir = (retreatPoint.position.x > transform.position.x) ? 1f : -1f;
                Vector2 targetVel = new Vector2(dir * retreatRunSpeed, 0f);
                transform.position = Vector2.MoveTowards(transform.position, retreatPoint.position, retreatRunSpeed * Time.deltaTime);
                spriteRenderer.flipX = dir < 0;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        if (gnomePrefab != null && retreatPoint != null)
        {
            GameObject newGnome = Instantiate(gnomePrefab, retreatPoint.position, Quaternion.identity);
            newGnome.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
            Rigidbody2D rb = newGnome.GetComponent<Rigidbody2D>();
            if (rb != null) Destroy(rb);
            Collider2D col = newGnome.GetComponent<Collider2D>();
            if (col != null) Destroy(col);
            Animator newAnim = newGnome.GetComponent<Animator>();
            if (newAnim != null) newAnim.Play("SitByTheFire", 0, 0f);
        }
        Destroy(gameObject);
    }
}
