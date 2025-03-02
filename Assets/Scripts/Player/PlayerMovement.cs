using System.Diagnostics.Contracts;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        #region Fields
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private Health healthUi;
        [Header("Movement Variables")]
        [SerializeField] private int direction = 1;
        [SerializeField] private float rollDuration = 8.0f / 14.0f;
        [SerializeField] private float rollCurrentDuration = 0f;
        [SerializeField] private bool grounded = false;
        [SerializeField] private bool attacking = false;
        [SerializeField] private bool rolling = false;
        [SerializeField] private bool controllable = true;
        [SerializeField] private bool blocking = false;
        [Header("Player Stats")]
        [SerializeField] private int currentAttack = 0;
        [SerializeField] private float attackDamage = 1;
        [SerializeField] private float attackCooldown = 0.0f;
        [SerializeField] private float attackRange = 2.05f;
        [SerializeField] private float health = 3.0f;
        [Header("Movement Forces")]
        [SerializeField] float speed = 5.0f;
        [SerializeField] float jumpForce = 3.0f;
        [SerializeField] float rollForce = 4.0f;
        #endregion

        #region Public Getters
        public int Direction => direction;
        public int CurrentAttack => currentAttack;
        public float AttackDamage => attackDamage;
        public float Health => health;
        public bool IsRolling => rolling;
        public bool IsAttacking => attacking;
        public bool IsGrounded => grounded;
        public bool Controllable => controllable;
        public bool Blocking => blocking;
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
            if (controllable)
            {
                #region Speed
                float inputX = Input.GetAxis("Horizontal");
                if (!rolling && !blocking)
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
                if (Input.GetMouseButtonDown(0) && attackCooldown > 0.25f && !rolling && !blocking)
                {
                    attacking = true;
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
                    blocking = true;
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    animator.SetBool("IdleBlock", false);
                    blocking = false;
                }
                #endregion

                #region Roll
                else if (Input.GetKeyDown(KeyCode.LeftShift) && !rolling && grounded && !blocking)
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
                #endregion
            }
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

        #region Damage Taking
        public void TakeDamage(float damage)
        {
            health -= damage;
            healthUi.UpdateHealth();
            if (health <= 0)
            {
                animator.SetTrigger("Die");
                controllable = false;
            }
            else animator.SetTrigger("Hurt");
        }
        #endregion
    }
}