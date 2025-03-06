using UnityEngine;

public class MushroomLogic : EnemyLogic
{
    private void Awake()
    {
        #region Component Retrieval
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        #endregion

        #region Base Variable Settings
        positionX = GetComponent<Transform>().position.x;
        startingPositionX = GetComponent<Transform>().position.x;
        patrolDistance = 1.5f;
        speed = 2.0f;
        attackRange = 2.05f;
        visionRange = 5.0f;
        attackResetCooldown = 1.2f;
        health = 2;
        damage = 1;
        pushforce = 5.0f;
        patrolling = true;
        damageable = true;
        canAttack = true;
        blockableAttack = true;
        #endregion

    }

}
