using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{   
    public EnemyData enemyData;

    #region Components
    Animator animator;
    EnemyStateManager enemyStateManager;
    public SpriteRenderer spriteRenderer;
    public BloodSplatRandomizer bloodSplatFX;
    public GameObject damageText;
    public GameObject deathPoof;
    #endregion
    
    #region EnemyData
    private int currentHealth;
    private int currentAttack;
    private float currentAttackRange;
    private float currentAttackWindupDuration;
    private float currentAttackRecoilDuration;
    private float currentAttackCooldown;
    private float currentStepDistance;
    private float currentStepDuration;
    private float currentStepInterval;
    #endregion

    #region Calculated Variables
    private float stepDurationTimer = 0;
    private float stepIntervalTimer = 0;
    private float attackCooldownTimer = 0;
    private float floorCoordinateY = 0;
    #endregion

    private GameObject target;
    public bool isInvincible;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyStateManager = GetComponent<EnemyStateManager>();
    }

    public void InitializeEnemy()
    {
        spriteRenderer.sprite = enemyData.enemySprite;

        // Set current stats from the enemyData base stats
        currentHealth = enemyData.baseHealth;
        currentAttack = enemyData.baseAttack;

        currentAttackRange = enemyData.baseAttackRange;
        currentAttackWindupDuration = enemyData.attackWindupDuration;
        currentAttackRecoilDuration = enemyData.attackRecoilDuration;
        currentAttackCooldown = enemyData.attackCooldown;

        currentStepDistance = enemyData.baseStepDistance;
        currentStepDuration = enemyData.baseStepDuration;
        currentStepInterval = enemyData.baseStepInterval;

        enemyStateManager.ChangeState(EnemyStateManager.EnemyState.Idle);
    }

    private void Update()
    {
        if (target == null || target.GetComponent<CharacterStateManager>().currentState == CharacterStateManager.CharacterState.Dead)
        {
            target = BattleManager.instance.GetClosestPlayerCharacter();
            return;
        }

        // Change behavior based on state
        switch (enemyStateManager.currentState)
        {
            case EnemyStateManager.EnemyState.Idle:
                if (Vector3.Distance(target.transform.position,transform.position) <= currentAttackRange)
                {   // Target is withing attack range
                    enemyStateManager.ChangeState(EnemyStateManager.EnemyState.Ready);
                }
                else 
                {
                    // Target is NOT within attack range
                    if (stepIntervalTimer <= 0)
                    {   // Can move
                        floorCoordinateY = transform.position.y;
                        animator.SetTrigger("Hop");
                        stepDurationTimer = currentStepDuration;
                        enemyStateManager.ChangeState(EnemyStateManager.EnemyState.Moving);
                    }
                    else
                    {   
                        stepIntervalTimer -= Time.deltaTime;
                    }
                }
                break;
            case EnemyStateManager.EnemyState.Moving:
                Step();
                break;
            case EnemyStateManager.EnemyState.Ready:
                if (attackCooldownTimer <= 0)
                {
                    StartCoroutine(AttackWindup());
                    enemyStateManager.ChangeState(EnemyStateManager.EnemyState.Attacking);
                }
                else
                {
                    attackCooldownTimer -= Time.deltaTime;
                }
                break;
            case EnemyStateManager.EnemyState.Attacking:
                // Attack
                break;
            default:
                enemyStateManager.ChangeState(EnemyStateManager.EnemyState.Idle);
                break;
        }
    }

    private void Step()
    {
        if (stepDurationTimer > 0)
        {
            // Calculate the progress of the step based on the remaining time
            float stepProgress = 1f - (stepDurationTimer / currentStepDuration); // From 0 to 1

            // Calculate the vertical position using a parabolic arc
            var newPosition = transform.position;
            newPosition.y = Mathf.Lerp(floorCoordinateY, floorCoordinateY + currentStepDuration, stepProgress * (1f - stepProgress) * 4);

            // Move horizontally towards the target
            transform.position = newPosition;
            transform.position += Vector3.left * currentStepDistance * Time.deltaTime;

            // Decrease the timer
            stepDurationTimer -= Time.deltaTime;
        }
        else 
        {
            // Set the final position back to the ground
            var newPosition = transform.position;
            newPosition.y = floorCoordinateY;
            transform.position = newPosition;

            // Reset the step interval and transition to Idle state
            stepIntervalTimer = currentStepInterval;
            animator.Play("enemy_land");
            enemyStateManager.ChangeState(EnemyStateManager.EnemyState.Idle);
        }
    }

    private IEnumerator AttackWindup()
    {
        animator.SetTrigger(enemyData.triggerAttackWindup);
        yield return new WaitForSeconds(currentAttackWindupDuration);
        StartCoroutine(AttackRelease());
    }

    private IEnumerator AttackRelease()
    {
        // If there is a target
        target = BattleManager.instance.GetClosestPlayerCharacter();
        if (target == null)
        {
            enemyStateManager.ChangeState(EnemyStateManager.EnemyState.Idle);
            animator.SetTrigger("Idle");
            attackCooldownTimer = currentAttackCooldown;
            yield break;
        }
        animator.SetTrigger(enemyData.triggerAttackRelease);
        switch (enemyData.attackType){
            case EnemyData.AttackType.instant:
                target.GetComponent<Character>().TakeDamage(currentAttack);
                break;
            case EnemyData.AttackType.projectile:
                break;
        }
        yield return new WaitForSeconds(currentAttackRecoilDuration);
        animator.SetTrigger(enemyData.triggerAttackReel);
        yield return new WaitForSeconds(0.5f);
        attackCooldownTimer = currentAttackCooldown;
        enemyStateManager.ChangeState(EnemyStateManager.EnemyState.Idle);
    }

    public void TakeDamage((float damageAmount, bool isCrit) damageInfo)
    {
        if (!gameObject.activeInHierarchy) {return;}
        DamagePopup damagePopup = Instantiate(damageText, 
                                              transform.position + new Vector3(0, 1), 
                                              Quaternion.identity)
                                                    .GetComponent<DamagePopup>();        
        damagePopup.Setup((int)damageInfo.damageAmount, damageInfo.isCrit);
        currentHealth -= (int)damageInfo.damageAmount;
        if (currentHealth <= 0 && !isInvincible)
        {
            Death();
        }
        else 
        {
            StartCoroutine(RedFlash());
            bloodSplatFX.PlayRandomSplat();
        }
    }

    IEnumerator RedFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = Color.white;
    }

    public void Death()
    {
        StopAllCoroutines();

        spriteRenderer.color = Color.white;
        spriteRenderer.transform.rotation = Quaternion.identity;
        spriteRenderer.transform.localScale = Vector3.one;

        bloodSplatFX.Reset();

        int randomInt = Random.Range(3, 4);
        float spawnRadius = 1.0f; // Define a radius around the enemy position

        for (int i = 0; i < randomInt; i++)
        {
            // Generate a random position within the spawn radius
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius; // Random point in a circle
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0); // Add offset to enemy position

            // When an enemy dies, spawn a coin
            GameObject coin = CoinPooler.instance.GetPooledCoin();
            if (coin != null)
            {
                // Activate and set up the coin to move to the currency icon
                coin.GetComponent<Coin>().ActivateCoin(spawnPosition, 10/randomInt + 1, (i + 1) * 0.05f);
            }
        }
        Destroy(Instantiate(deathPoof,
                            transform.position + new Vector3(0, 1), 
                            Quaternion.identity), 1.0f);


        EnemyPooler.instance.ReturnEnemyToPool(gameObject);
    }
}
