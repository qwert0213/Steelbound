using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    #region Fields
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Health healthUi;
    [SerializeField] private DeathMenu deathMenu;
    public SettingsManager settingsManager;
    [Header("Movement Variables")]
    [SerializeField] private int direction = 1;
    [SerializeField] private float rollDuration = 8.0f / 14.0f;
    [SerializeField] private float rollCurrentDuration = 0f;
    [SerializeField] private float immuneDuration = 0.2f;
    [SerializeField] private float immuneCurrentDuartion = 0f;
    [SerializeField] private bool grounded = false;
    [SerializeField] private bool attacking = false;
    [SerializeField] private bool rolling = false;
    [SerializeField] public bool controllable = true;
    [SerializeField] private bool blocking = false;
    [SerializeField] private bool interacting = false;
    [SerializeField] private bool immune = false;
    [Header("Player Stats")]
    [SerializeField] private int currentAttack = 0;
    [SerializeField] private float attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.0f;
    [SerializeField] private float attackRange = 2.05f;
    [SerializeField] private float maxHealth = 3.0f;
    [SerializeField] private float health = 3.0f;
    [Header("Movement Forces")]
    [SerializeField] float speed = 5.0f;
    [SerializeField] float jumpForce = 3.0f;
    [SerializeField] float rollForce = 4.0f;
    [Header("Level Modifiers")]
    public bool isSlippery = false;
    #endregion

    #region Public Getters
    public int Direction => direction;
    public int CurrentAttack => currentAttack;
    public float AttackDamage => attackDamage;
    public float Health => health;
    public float MaxHealth => maxHealth;
    public bool IsRolling => rolling;
    public bool IsAttacking => attacking;
    public bool IsGrounded => grounded;
    public bool Controllable => controllable;
    public bool Blocking => blocking;
    public bool Interacting => interacting;
    public float RollDuration => rollDuration;
    public float Speed => speed;
    public float RollForce => rollForce;
    public float JumpForce => jumpForce;
    public float AttackRange => attackRange;
    public Animator Animator => animator;
    public Rigidbody2D Body => body;
    #endregion
    void Awake()
    {
        #region Component Retrieval
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        healthUi = GameObject.FindGameObjectWithTag("Healthbar").GetComponent<Health>();
        deathMenu = GameObject.FindGameObjectWithTag("Settings").GetComponent<DeathMenu>();
        string controllerPath = "Assets/Animations/Player/Player.controller";
        RuntimeAnimatorController animatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
        if (animatorController != null)
        {
            animator.runtimeAnimatorController = animatorController;
        }
        else
        {
            Debug.LogError("Animator Controller could not be loaded from path: " + controllerPath);
        }
        settingsManager = SettingsManager.Instance;
        #endregion
    }

    void Update()
    {
        if (controllable)
        {
            #region Speed
            float inputX = Input.GetAxisRaw("Horizontal");
            CheckGrounded();

            float targetSpeed = inputX * speed;
            float acceleration;
            float deceleration;

            if (isSlippery)
            {
                acceleration = 2f;
                deceleration = 0.5f;
            }
            else
            {
                acceleration = 8f;
                deceleration = 6f;
            }

            if (!rolling && !blocking && !immune)
            {
                float velocityX = body.linearVelocity.x;

                if (Mathf.Abs(inputX) > 0.01f)
                {
                    velocityX = Mathf.MoveTowards(velocityX, targetSpeed, acceleration * Time.deltaTime);
                }
                else
                {
                    velocityX = Mathf.MoveTowards(velocityX, 0, deceleration * Time.deltaTime);
                }

                body.linearVelocity = new Vector2(velocityX, body.linearVelocity.y);
            }

            animator.SetFloat("AirSpeedY", body.linearVelocity.y);
            #endregion

            #region Direction
            if (inputX > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
                direction = 1;
            }
            else if (inputX < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                direction = -1;
            }
            #endregion

            #region Run
            if (Mathf.Abs(inputX) > 0 && !rolling && !blocking)
            {
                animator.SetBool("Running", true);
            }
            else
            {
                animator.SetBool("Running", false);
            }
            #endregion

            #region Attack
            if (Input.GetKeyDown(settingsManager.GetKeyForAction("Attack")) && attackCooldown > 0.25f && !rolling && !blocking)
            {
                attacking = true;
                currentAttack++;
                if (currentAttack > 3) currentAttack = 1;
                if (attackCooldown > 1.0f) currentAttack = 1;
                animator.SetTrigger("Attack" + currentAttack);
                attackCooldown = 0.0f;
                AudioManager.Instance.PlaySFX(AudioManager.Instance.playerAttack);
            }
            #endregion

            #region Block
            else if (Input.GetKeyDown(settingsManager.GetKeyForAction("Block")) && !rolling)
            {
                animator.SetTrigger("Block");
                animator.SetBool("IdleBlock", true);
                blocking = true;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                animator.SetBool("IdleBlock", false);
                blocking = false;
            }
            #endregion

            #region Roll
            else if (Input.GetKeyDown(settingsManager.GetKeyForAction("Roll")) && !rolling && grounded && !blocking)
            {
                rolling = true;
                rollCurrentDuration = 0.0f;
                animator.SetTrigger("Roll");
                body.linearVelocity = new Vector2(direction * rollForce, body.linearVelocity.y);
            }
            #endregion

            #region Jump
            else if (Input.GetKeyDown(KeyCode.Space) && grounded && !rolling && !blocking)
            {
                animator.SetTrigger("Jump");
                grounded = false;
                animator.SetBool("Grounded", grounded);
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
            }
            #endregion

            #region Interact
            if (Input.GetKeyDown(settingsManager.GetKeyForAction("Interact")) && !rolling && !blocking) interacting = true;
            else interacting = false;
            #endregion

            #region Cooldowns
            attackCooldown += Time.deltaTime;
            if (attackCooldown > 5 / 14f)
            {
                attacking = false;
            }
            if (rolling)
            {
                rollCurrentDuration += Time.deltaTime;
            }
            if (rollCurrentDuration > rollDuration)
            {
                rolling = false;
            }
            if (immune)
            {
                immuneCurrentDuartion += Time.deltaTime;
            }
            if (immuneCurrentDuartion > immuneDuration)
            {
                immune = false;
            }
            #endregion
        }
    }
    #region Grounded Detection
    private void CheckGrounded()
    {
        float rayLength = 0.01f;
        float rayOffset = boxCollider.bounds.extents.x * 0.9f;
        Vector2 leftRayOrigin = new Vector2(transform.position.x - rayOffset, transform.position.y - boxCollider.bounds.extents.y - 0.05f);
        Vector2 centerRayOrigin = new Vector2(transform.position.x, transform.position.y - boxCollider.bounds.extents.y - 0.05f);
        Vector2 rightRayOrigin = new Vector2(transform.position.x + rayOffset, transform.position.y - boxCollider.bounds.extents.y - 0.05f);

        if (body.linearVelocity.y > 0)
        {
            grounded = false;
            animator.SetBool("Grounded", false);
            return;
        }

        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, rayLength, LayerMask.GetMask("Ground"));
        RaycastHit2D centerHit = Physics2D.Raycast(centerRayOrigin, Vector2.down, rayLength, LayerMask.GetMask("Ground"));
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, rayLength, LayerMask.GetMask("Ground"));
        grounded = leftHit.collider != null || centerHit.collider != null || rightHit.collider != null;

        animator.SetBool("Grounded", grounded);
    }
    #endregion

    #region Damage Taking
    public void TakeDamage(float damage, float pushForce)
    {
        if (!immune)
        {
            health -= damage;
            healthUi.UpdateHealth();
            immune = true;
            immuneCurrentDuartion = 0;
            if (controllable)
            {
                if (health <= 0)
                {
                    animator.SetTrigger("Die");
                    controllable = false;
                    deathMenu.ShowDeathMenu();
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.playerDeath);
                }
                else
                {
                    Vector2 pushDirection = new Vector2(-direction * pushForce, 1.3f);
                    body.linearVelocity = Vector2.zero;
                    body.AddForce(pushDirection, ForceMode2D.Impulse);
                    animator.SetTrigger("Hurt");
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.playerHurt);
                }
            }
        }
    }
    #endregion
    #region Heal
    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        healthUi.UpdateHealth();
    }
    #endregion

    #region Collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fire"))
        {
            TakeDamage(1, 7f);
        }
    }
    #endregion
}