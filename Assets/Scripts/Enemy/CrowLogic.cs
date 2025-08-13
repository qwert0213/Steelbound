using UnityEngine;

public class CrowLogic : EnemyLogic
{
    public enum CrowState { Idle, GoToPlayer, Grab, Ascend, CarryToTarget, Release }

    public CrowState state = CrowState.Idle;

    [Header("References")]
    public Transform playerPosition;
    public Transform ascendPoint; // poz�ci�, ahova el�sz�r felrep�l
    public Transform dropPoint;   // c�lpont, ahova elviszi a j�t�kost

    [Header("Settings")]
    public float flySpeed = 5f;
    public float grabDistance = 1f;
    public Vector3 carryOffset = new Vector3(0, 1.5f, 0); // j�t�kos f�l� rep�l�s

    private Animator anim;

    void Start()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        switch (state)
        {
            case CrowState.Idle:
                anim.SetTrigger("Idle");
                break;

            case CrowState.GoToPlayer:
                anim.SetTrigger("Fly");
                MoveTowards(playerPosition.position + carryOffset);
                if (Vector3.Distance(transform.position, playerPosition.position + carryOffset) <= grabDistance)
                {
                    GrabPlayer();
                }
                break;

            case CrowState.Grab:
                // R�vid v�rakoz�s/anim�ci� lehet, majd tov�bbl�p�s
                state = CrowState.Ascend;
                break;

            case CrowState.Ascend:
                anim.SetTrigger("Carry");
                MoveTowards(ascendPoint.position);
                UpdateCarriedPlayerPosition();
                if (Vector3.Distance(transform.position, ascendPoint.position) <= 0.1f)
                {
                    state = CrowState.CarryToTarget;
                }
                break;

            case CrowState.CarryToTarget:
                MoveTowards(dropPoint.position);
                UpdateCarriedPlayerPosition();
                if (Vector3.Distance(transform.position, dropPoint.position) <= 0.1f)
                {
                    ReleasePlayer();
                }
                break;

            case CrowState.Release:
                anim.SetTrigger("Idle");
                break;
        }
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * flySpeed * Time.deltaTime;
        if (dir.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(dir.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void GrabPlayer()
    {
        state = CrowState.Grab;
        // Letiltja a j�t�kos ir�ny�t�s�t
        if (player != null)
            player.controllable = false;
    }

    void UpdateCarriedPlayerPosition()
    {
        if (player != null)
            playerPosition.position = transform.position - carryOffset;
    }

    void ReleasePlayer()
    {
        if (player != null)
            player.controllable = true; // visszaadja az ir�ny�t�st
        state = CrowState.Release;
    }

    // Ezt h�vod meg k�v�lr�l, amikor a j�t�kos felveszi a kulcsot
    public void OnPlayerPickedKey()
    {
        if (state == CrowState.Idle)
            state = CrowState.GoToPlayer;
    }
}
