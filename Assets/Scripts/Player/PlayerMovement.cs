using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Fields
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private BoxCollider2D boxCollider;
    [Header("Movement Variables")]
    [SerializeField] private int direction = 1;
    [SerializeField] private int currentAttack = 0;
    [SerializeField] private float attackCooldown = 0.0f;
    [SerializeField] private float rollDuration = 8.0f / 14.0f;
    [SerializeField] private float rollCurrentDuration = 0f;
    [SerializeField] private bool grounded = false;
    [SerializeField] private bool rolling = false;
    [Header("Movement Forces")]
    [SerializeField] float speed = 5.0f;
    [SerializeField] float jumpForce = 3.0f;
    [SerializeField] float rollForce = 4.0f;
    #endregion

    #region Public Getters
    public int Direction => direction;
    public int CurrentAttack => currentAttack;
    public bool IsRolling => rolling;
    public bool IsGrounded => grounded;
    public float RollDuration => rollDuration;
    public float Speed => speed;
    public float RollForce => rollForce;
    public float JumpForce => jumpForce;
    public Animator Animator => animator;
    #endregion
    void Awake()
    {
        #region Component Retrieval
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
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
        #endregion
    }

    void Update()
    {
        #region Speed
        float inputX = Input.GetAxis("Horizontal");
        if (!rolling)
        {
            body.linearVelocity = new Vector2(inputX * speed, body.linearVelocity.y);
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
        if (Mathf.Abs(inputX) > 0 && !rolling)
        {
            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Running", false);
        }
        #endregion

        #region Attack
        if (Input.GetMouseButtonDown(0) && attackCooldown > 0.25f && !rolling)
        {
            currentAttack++;
            if (currentAttack > 3) currentAttack = 1;
            if (attackCooldown > 1.0f) currentAttack = 1;
            animator.SetTrigger("Attack" + currentAttack);
            attackCooldown = 0.0f;
        }
        #endregion

        #region Block
        else if (Input.GetMouseButtonDown(1) && !rolling)
        {
            animator.SetTrigger("Block");
            animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("IdleBlock", false);
        }
        #endregion

        #region Roll
        else if (Input.GetKeyDown(KeyCode.LeftShift) && !rolling && grounded)
        {
            rolling = true;
            rollCurrentDuration = 0.0f;
            animator.SetTrigger("Roll");
            body.linearVelocity = new Vector2(direction * rollForce, body.linearVelocity.y);
        }
        #endregion

        #region Jump
        else if (Input.GetKeyDown(KeyCode.Space) && grounded && !rolling)
        {
            animator.SetTrigger("Jump");
            grounded = false;
            animator.SetBool("Grounded", grounded);
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        }
        #endregion

        #region Cooldowns
        attackCooldown += Time.deltaTime;
        if (rolling)
        {
            rollCurrentDuration += Time.deltaTime;
        }
        if (rollCurrentDuration > rollDuration)
        {
            rolling = false;
        }
        #endregion
    }
    #region Collision Detection
    private void OnCollisionStay2D(Collision2D collision) // Grounded = true
    {
        if (collision.collider.CompareTag("ground"))
        {
            grounded = true;
            animator.SetBool("Grounded", grounded);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) // Grounded = false
    {
        if (collision.collider.CompareTag("ground"))
        {
            grounded = false;
            animator.SetBool("Grounded", grounded);
        }
    }
    #endregion
}