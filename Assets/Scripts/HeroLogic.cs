using System.Collections;
using UnityEngine;

public class HeroLogic : EnemyLogic
{
    [Header("Hero Specific")]
    [SerializeField] private Transform retreatPoint1;
    [SerializeField] private Transform retreatPoint2;
    [SerializeField] private float retreatSpeed = 2f;
    [SerializeField] private float fastRetreatSpeed = 6f;
    [SerializeField] private float retreatTime = 1.5f;
    [SerializeField] private int hitsBeforeRetreat = 3;
    [SerializeField] private float followSpeed = 3f;

    private SpriteRenderer spriteRenderer;
    private int currentPhase = 0;
    private int damageTakenInPhase = 0;
    private bool isAttacking = false;

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
        health = 10;
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


            if (distToPlayer < visionRange && distToPlayer > attackRange)
            {
                FollowPlayer();
            }

            else if (distToPlayer <= attackRange && canAttack)
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
        bool facingEnemy = (player.Direction == 1 && dirToEnemy.x > 0) ||
                           (player.Direction == -1 && dirToEnemy.x < 0);

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
            if ((player.Direction == 1 && player.transform.position.x < transform.position.x) ||
                (player.Direction == -1 && player.transform.position.x > transform.position.x))
            {
                health -= player.AttackDamage;
                damageTakenInPhase++;

                    damageable = false;
                    animator.SetBool("Damageable", false);
                    animator.SetTrigger("Hit");
                    damageCooldown = 0;
                    PlayHurtSound();

                    if (damageTakenInPhase >= hitsBeforeRetreat)
                    {
                        damageTakenInPhase = 0;
                        currentPhase++;
                        if (currentPhase == 1)
                            StartCoroutine(RunToPosition(retreatPoint1.position));
                        else if (currentPhase == 2)
                            StartCoroutine(RunToPosition(retreatPoint2.position));
                            FinalRetreat();
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

    private IEnumerator RunToPosition(Vector3 targetPos)
    {
        overrideControl = true;
        animator.SetTrigger("Run");

        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            float dir = (targetPos.x > transform.position.x) ? 1f : -1f;
            Vector2 targetVel = new Vector2(dir * fastRetreatSpeed, 1f); 
            body.linearVelocity = Vector2.Lerp(body.linearVelocity, targetVel, Time.deltaTime * 10f);
            spriteRenderer.flipX = dir < 0;
            yield return null;
        }


        body.linearVelocity = Vector2.zero;
        animator.SetTrigger("Idle");
        yield return new WaitForSeconds(1f);

        overrideControl = false;
        damageable = true;
    }

    private void FinalRetreat()
    {
        overrideControl = true;
        animator.SetTrigger("Die");
        body.linearVelocity = Vector2.zero;
    }
}
