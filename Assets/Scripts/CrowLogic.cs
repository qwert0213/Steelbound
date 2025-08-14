using System.Collections;
using UnityEngine;

public class CrowLogic : EnemyLogic
{
    #region Fields
    public enum CrowState { Idle, GoToPlayer, Grab, Ascend, CarryToTarget, Release }
    public CrowState state = CrowState.Idle;

    [Header("References")]
    public Transform ascendPoint;
    public Transform dropPoint;

    [Header("Settings")]
    public float flySpeed = 5f;
    public float grabDistance = 10f;
    public Vector3 carryOffset = new Vector3(0, 1.5f, 0);

    [Header("Child References")]
    public Transform spriteHolder;
    public Transform carryPoint;

    [Header("Internals")]
    private CrowState lastState;
    private bool carrying;
    private bool useParenting = false;
    private RigidbodyType2D crowBodyPrevType;
    private float crowBodyPrevGravity;
    private SpriteRenderer[] crowSRs;
    private int[] crowSR_orderBackup;
    private SpriteMaskInteraction[] crowSR_maskBackup;

    [Header("Player")]
    private Transform playerT;
    private Rigidbody2D playerRB;
    private bool playerHadParent;
    private Transform playerPrevParent;
    private RigidbodyType2D playerPrevType;
    #endregion
    void Start()
    {
        #region Assignments
        if (animator == null) animator = GetComponentInChildren<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerT = player.transform;
        playerRB = player.GetComponent<Rigidbody2D>();

        lastState = state;
        SetAnimTrigger("Idle");

        crowSRs = GetComponentsInChildren<SpriteRenderer>(true);
        crowSR_orderBackup = new int[crowSRs.Length];
        crowSR_maskBackup = new SpriteMaskInteraction[crowSRs.Length];
        for (int i = 0; i < crowSRs.Length; i++)
        {
            crowSR_orderBackup[i] = crowSRs[i].sortingOrder;
            crowSR_maskBackup[i] = crowSRs[i].maskInteraction;
        }

        if (carryPoint != null)
        {
            var p = carryPoint.position;
            carryPoint.position = new Vector3(p.x, p.y, 0f);
        }
        #endregion
    }

    void Update()
    {
        #region States
        if (state != lastState)
        {
            OnStateChanged(lastState, state);
            lastState = state;
        }

        switch (state)
        {
            case CrowState.Idle:
                break;

            case CrowState.GoToPlayer:
                AudioManager.Instance.PlaySFX(AudioManager.Instance.crowSay);
                MoveTowards(playerT.position + carryOffset);
                if (Vector3.Distance(transform.position, playerT.position + carryOffset) <= grabDistance)
                    GrabPlayer();
                break;

            case CrowState.Grab:
                AudioManager.Instance.PlaySFX(AudioManager.Instance.crowSay);
                break;

            case CrowState.Ascend:
                MoveTowards(ascendPoint.position);
                if (!useParenting && carrying) FollowCarryPoint();
                if (Vector3.Distance(transform.position, ascendPoint.position) <= 0.1f)
                    state = CrowState.CarryToTarget;
                break;

            case CrowState.CarryToTarget:
                AudioManager.Instance.PlaySFX(AudioManager.Instance.crowSay);
                MoveTowards(dropPoint.position);
                if (!useParenting && carrying) FollowCarryPoint();
                if (Vector3.Distance(transform.position, dropPoint.position) <= 0.1f)
                    ReleasePlayer();
                break;

            case CrowState.Release:
                break;
        }
        #endregion
    }


    #region Pickup Player
    void MoveTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        float wave = Mathf.Sin(Time.time * 4f) * 1f;
        Vector3 waveOffset = new Vector3(0, wave, 0);

        transform.position += (dir * flySpeed * Time.deltaTime) + waveOffset * Time.deltaTime;

        if (dir.x != 0 && spriteHolder != null)
        {
            spriteHolder.localScale = new Vector3(Mathf.Sign(dir.x) * Mathf.Abs(spriteHolder.localScale.x), spriteHolder.localScale.y, spriteHolder.localScale.z);
        }
    }
    #endregion

    #region Position Follow
    void LateUpdate()
    {
        if (!useParenting && carrying) FollowCarryPoint();
    }
    void FollowCarryPoint(bool snap = false)
    {
        if (playerT == null || carryPoint == null) return;

        if (snap)
        {
            playerT.position = carryPoint.position;
        }
        else
        {
            playerT.position = carryPoint.position;
        }

        if (Mathf.Abs(playerT.position.z) > 0.0001f)
        {
            var p = playerT.position; playerT.position = new Vector3(p.x, p.y, 0f);
        }
    }
    #endregion

    #region Player Manipulation
    void GrabPlayer()
    {
        state = CrowState.Grab;
        overrideControl = true;
        if (body != null)
        {
            crowBodyPrevType = body.bodyType;
            crowBodyPrevGravity = body.gravityScale;
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }

        for (int i = 0; i < crowSRs.Length; i++)
        {
            crowSRs[i].maskInteraction = SpriteMaskInteraction.None;
            crowSRs[i].sortingOrder = crowSR_orderBackup[i] + 500;
        }

        player.controllable = false;
        player.Animator.SetFloat("AirSpeedY", -1);
        player.Animator.SetBool("Grounded", false);

        if (playerRB != null)
        {
            playerPrevType = playerRB.bodyType;
            playerRB.linearVelocity = Vector2.zero;
            playerRB.bodyType = RigidbodyType2D.Kinematic;
        }

        playerHadParent = playerT.parent != null;
        playerPrevParent = playerT.parent;

        if (useParenting)
        {
            playerT.SetParent(carryPoint, worldPositionStays: false);
            playerT.localPosition = Vector3.zero;
        }
        else
        {
            playerT.SetParent(null, true);
            FollowCarryPoint(true);
        }

        carrying = true;
        StartCoroutine(StartAscendNextFrame());
    }
    void ReleasePlayer()
    {
        overrideControl = false;
       if (useParenting) playerT.SetParent(null, true);
        else if (playerHadParent && playerPrevParent != null) playerT.SetParent(playerPrevParent, true);

        if (playerRB != null)
        {
            playerRB.bodyType = playerPrevType;
        }
        player.controllable = true;
        player.Animator.SetFloat("AirSpeedY", 0);
        player.Animator.SetBool("Grounded", true);
        if (body != null)
        {
            body.bodyType = crowBodyPrevType;
            body.gravityScale = crowBodyPrevGravity;
        }

        for (int i = 0; i < crowSRs.Length; i++)
        {
            crowSRs[i].maskInteraction = crowSR_maskBackup[i];
            crowSRs[i].sortingOrder = crowSR_orderBackup[i];
        }

        carrying = false;
        state = CrowState.Release;


    }


    protected IEnumerator StartAscendNextFrame()
    {
        yield return null;
        state = CrowState.Ascend;
    }
 
    public void OnPlayerPickedKey()
    {
        if (state == CrowState.Idle)
            state = CrowState.GoToPlayer;
    }
    #endregion

    #region Animations
    private void OnStateChanged(CrowState oldState, CrowState newState)
    {
        switch (newState)
        {
            case CrowState.Idle:
            case CrowState.Release:
                AudioManager.Instance.StopLoopSFX();
                SetAnimTrigger("Idle");
                break;

            case CrowState.GoToPlayer:
            case CrowState.Ascend:
            case CrowState.CarryToTarget:
                AudioManager.Instance.PlayLoopSFX(AudioManager.Instance.crowFly);
                SetAnimTrigger("Fly");
                break;
        }
    }


    private void SetAnimTrigger(string trig)
    {
        if (animator != null) animator.SetTrigger(trig);
    }
    #endregion
}
