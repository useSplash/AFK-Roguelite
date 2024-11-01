using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterData characterData;
    public SpriteRenderer characterSpriteRenderer;
    public Healthbar healthbar;
    public ParticleSystem buffParticleSystem;
    public GameObject healingAnimation;
    public BurstButton burstButton;

    #region Components
    Animator animator;
    CharacterStateManager characterStateManager;
    #endregion

    #region CharacterData
    // Character stats
    private int currentHealth;
    private int currentAttackPower;
    private int currentDefense;
    private float currentCritChance;
    private float currentCritDamage;
    private float currentSpeed; // All characters start at 1
    
    // Character basic attack variables
    private float currentBasicAttackWindupDuration;
    private float currentBasicAttackRecoilDuration;

    // Character special ability variables
    private float currentSpecialAbilityWindupDuration;
    private float currentSpecialAbilityRecoilDuration;
    private float currentSpecialAbilityCooldown;

    // Character burst ability variables
    private float currentBurstAbilityWindupDuration;
    private float currentBurstAbilityRecoilDuration;
    #endregion
    
    #region Buff Information
    private List<BuffData> activeBuffs = new List<BuffData>();
    private Dictionary<BuffData, float> buffTimers = new Dictionary<BuffData, float>();
    #endregion

    private GameObject target;
    private float specialAbilityCooldownTimer;
    private float currentEnergy = 0;
    private float currentMaxEnergy = 100;
    private bool waveInProgress = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterStateManager = GetComponent<CharacterStateManager>();

        healingAnimation.SetActive(false);
    }

    public void InitializeCharacter()
    {
        characterSpriteRenderer.sprite = characterData.characterSprite;

        // Initialize stats from CharacterData
        currentHealth = characterData.baseHealth;
        currentAttackPower = characterData.baseAttackPower;
        currentDefense = characterData.baseDefense;
        currentCritChance = characterData.baseCritChance;
        currentCritDamage = characterData.baseCritDamage;
        currentSpeed = 1.0f; // Added stat

        currentBasicAttackWindupDuration = characterData.basicAttackWindupDuration;
        currentBasicAttackRecoilDuration = characterData.basicAttackRecoilDuration;

        currentSpecialAbilityWindupDuration = characterData.specialAbilityWindupDuration;
        currentSpecialAbilityRecoilDuration = characterData.specialAbilityRecoilDuration;
        currentSpecialAbilityCooldown = characterData.specialAbilityCooldown;

        currentBurstAbilityWindupDuration = characterData.burstAbilityWindupDuration;
        currentBurstAbilityRecoilDuration = characterData.burstAbilityRecoilDuration;
        
        characterStateManager.ChangeState(CharacterStateManager.CharacterState.Idle);
        healthbar.UpdateValues(currentHealth, characterData.baseHealth);
        specialAbilityCooldownTimer = currentSpecialAbilityCooldown;
    }

    void Update()
    {   
        // Update the buff timers
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            BuffData buff = activeBuffs[i];
            if (waveInProgress) buffTimers[buff] -= Time.deltaTime;

            if (buffTimers[buff] <= 0)
            {
                RemoveBuff(buff);
                activeBuffs.RemoveAt(i);
            }
        }

        #region  Buff Particle System
        
        // Get the Emission module from the ParticleSystem
        var emission = buffParticleSystem.emission;

        // Get the Main module from the ParticleSystem
        var main = buffParticleSystem.main;

        Color buffColor1 = Color.white;
        Color buffColor2 = Color.white;

        if (activeBuffs.Count == 0)
        {
            // Case when no buffs are active
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(0);
        }
        else if (activeBuffs.Count >= 1)
        {
            // Case when 1 buff is active
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(20);
            switch (activeBuffs[0].statType){
                case BuffData.StatType.Attack:
                    buffColor1 = Color.red;
                    break;
                case BuffData.StatType.Defense:
                    buffColor1 = Color.blue;
                    break;
                case BuffData.StatType.Speed:
                    buffColor1 = Color.yellow;
                    break;
            }
            buffColor2 = buffColor1;
            if (activeBuffs.Count >= 2){
                int counter = 1;
                while (buffColor1 == buffColor2 && counter < activeBuffs.Count)
                {
                    // Case when 2 or more buffs are active
                    emission.rateOverTime = new ParticleSystem.MinMaxCurve(50);
                    switch (activeBuffs[counter].statType){
                        case BuffData.StatType.Attack:
                            buffColor2 = Color.red;
                            break;
                        case BuffData.StatType.Defense:
                            buffColor2 = Color.blue;
                            break;
                        case BuffData.StatType.Speed:
                            buffColor2 = Color.yellow;
                            break;
                    }
                    counter += 1;
                }
            }
            main.startColor = new ParticleSystem.MinMaxGradient(buffColor1, buffColor2);
        }
        #endregion

        // Update special ability timer
        if (waveInProgress) specialAbilityCooldownTimer -= Time.deltaTime;
        if (specialAbilityCooldownTimer < 0) specialAbilityCooldownTimer = 0;

        // Change behavior based on state
        switch (characterStateManager.currentState)
        {
            case CharacterStateManager.CharacterState.Idle:
                if (BattleManager.instance.GetEnemies().Length > 0){
                    if (specialAbilityCooldownTimer <= 0){
                        StartCoroutine(SpecialAbilityWindup());
                        characterStateManager.ChangeState(CharacterStateManager.CharacterState.Attacking);
                    }
                    else {
                        StartCoroutine(BasicAttackWindup());
                        characterStateManager.ChangeState(CharacterStateManager.CharacterState.Attacking);
                    }
                }
                break;
            case CharacterStateManager.CharacterState.Attacking:
                break;
            case CharacterStateManager.CharacterState.Burst:
                break;
            case CharacterStateManager.CharacterState.Stunned:
                break;
            case CharacterStateManager.CharacterState.Dead:
                break;
            default:
                break;
        }
    }
    
    #region Basic Attack Animation
    private IEnumerator BasicAttackWindup()
    {
        animator.SetTrigger(characterData.triggerBasicAttackWindup);
        yield return new WaitForSeconds(currentBasicAttackWindupDuration / currentSpeed);
        if (BattleManager.instance.GetClosestEnemy(transform.position) == null) {
            characterStateManager.ChangeState(CharacterStateManager.CharacterState.Idle);
            animator.SetTrigger("Idle");
            yield break;
        }
        StartCoroutine(BasicAttackRelease());
    }

    private IEnumerator BasicAttackRelease()
    {
        // If there is a target
        target = BattleManager.instance.GetClosestEnemy(transform.position);
        if (target == null)
        {
            characterStateManager.ChangeState(CharacterStateManager.CharacterState.Idle);
            animator.SetTrigger("Idle");
            yield break;
        }

        GameObject pfAttack = characterData.pfBasicAttack;
        if (pfAttack.GetComponent<Damage>() != null)
        {
            pfAttack.GetComponent<Damage>().Setup(currentAttackPower, 
                                                characterData.basicAttackDamageMultiplier,
                                                currentCritChance,
                                                currentCritDamage);
        }
        animator.SetTrigger(characterData.triggerBasicAttackRelease);
        float targetOffsetY = target.GetComponent<Collider2D>().bounds.size.y/3;
        if (targetOffsetY == 0)
        {
            targetOffsetY = 1.5f;
        }
        else {
            targetOffsetY += 0.5f;
        }

        // Perform Attack
        ExecuteAbility(characterData.basicAttackType, pfAttack, target, targetOffsetY);
        GainEnergy(10);

        yield return new WaitForSeconds(currentBasicAttackRecoilDuration / currentSpeed);
        animator.SetTrigger(characterData.triggerBasicAttackReel);
        yield return new WaitForSeconds(0.5f / currentSpeed);
        characterStateManager.ChangeState(CharacterStateManager.CharacterState.Idle);
    }
    #endregion

    #region Special Ability Animation
    private IEnumerator SpecialAbilityWindup()
    {
        animator.SetTrigger(characterData.triggerSpecialAbilityWindup);
        yield return new WaitForSeconds(currentSpecialAbilityWindupDuration / currentSpeed);
        if (BattleManager.instance.GetClosestEnemy(transform.position) == null) {
            characterStateManager.ChangeState(CharacterStateManager.CharacterState.Idle);
            animator.SetTrigger("Idle");
            yield break;
        }
        StartCoroutine(SpecialAbilityRelease());
    }

    private IEnumerator SpecialAbilityRelease()
    {
        // If there is a target
        target = BattleManager.instance.GetClosestEnemy(transform.position);
        if (target == null)
        {
            characterStateManager.ChangeState(CharacterStateManager.CharacterState.Idle);
            animator.SetTrigger("Idle");
            yield break;
        }

        // Perform Attack
        GameObject pfAbility = characterData.pfSpecialAbility;
        if (pfAbility.GetComponent<Damage>() != null)
        {
            pfAbility.GetComponent<Damage>().Setup(currentAttackPower, 
                                                characterData.specialAbilityDamageMultiplier,
                                                currentCritChance,
                                                currentCritDamage);
        }
        animator.SetTrigger(characterData.triggerSpecialAbilityRelease);
        float targetOffsetY = target.GetComponent<Collider2D>().bounds.size.y/3;
        if (targetOffsetY == 0)
        {
            targetOffsetY = 1.5f;
        }
        else 
        {
            targetOffsetY += 0.5f;
        }

        // Execute Special Ability
        ExecuteAbility(characterData.specialAbilityType, pfAbility, target, targetOffsetY);

        // Reel After Ability
        specialAbilityCooldownTimer = currentSpecialAbilityCooldown;
        yield return new WaitForSeconds(currentSpecialAbilityRecoilDuration / currentSpeed);
        animator.SetTrigger(characterData.triggerSpecialAbilityReel);
        yield return new WaitForSeconds(0.5f / currentSpeed);
        characterStateManager.ChangeState(CharacterStateManager.CharacterState.Idle);
    }
    #endregion
    
    #region Burst Ability Animation
    public void ActivateBurst()
    {
        // Don't activate if not enough energy or dead
        if (currentEnergy < currentMaxEnergy 
            || IsDead() 
            || !waveInProgress) return;
        
        StopAllCoroutines();
        GainEnergy(-currentEnergy);
        characterStateManager.ChangeState(CharacterStateManager.CharacterState.Burst);
        BurstAnimationHandler.instance.PlayBurstAnimation(characterData.burstAbilityName, 
                                                          characterData.characterSprite, 
                                                          () => {
            // Start Burst Ability
            StartCoroutine(BurstAbilityRelease());
        });
    }
    private IEnumerator BurstAbilityRelease()
    {
        // Perform Attack
        GameObject pfAbility = characterData.pfBurstAbility;
        if (pfAbility.GetComponent<Damage>() != null)
        {
            pfAbility.GetComponent<Damage>().Setup(currentAttackPower, 
                                                characterData.burstAbilityDamageMultiplier,
                                                currentCritChance,
                                                currentCritDamage);
        }
        float targetOffsetY = target.GetComponent<Collider2D>().bounds.size.y/3;
        if (targetOffsetY == 0)
        {
            targetOffsetY = 1.5f;
        }
        else 
        {
            targetOffsetY += 0.5f;
        }

        Vector3 originalPosition = transform.position;

        if (characterData.travelToCenter)
        {
            Vector3 targetPosition = new Vector3(0, -2.5f);
            float reachedDistance = 0.5f;
            float speed = 30.0f;

            animator.Play("character_dash");

            while (Vector3.Distance(transform.position, targetPosition) > reachedDistance) 
            {
                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;

                if (Vector3.Distance(transform.position, targetPosition) < reachedDistance)
                {
                    transform.position = targetPosition;  // Snap to the exact position
                }
                yield return null;  // Wait for the next frame
            }
        }
        
        // Windup Burst
        animator.SetTrigger(characterData.triggerBurstAbilityWindup);
        yield return new WaitForSeconds(currentBurstAbilityWindupDuration);

        // Execute Burst
        animator.SetTrigger(characterData.triggerBurstAbilityRelease);
        ExecuteAbility(characterData.burstAbilityType, pfAbility, target, targetOffsetY);

        // Reel
        yield return new WaitForSeconds(currentBurstAbilityRecoilDuration);
        animator.SetTrigger(characterData.triggerBurstAbilityReel);
        yield return new WaitForSeconds(0.5f);

        // Dash back
        if (characterData.travelToCenter)
        {
            Vector3 targetPosition = originalPosition;
            float reachedDistance = 0.5f;
            float speed = 30.0f;
            animator.Play("character_dash_back");

            while (Vector3.Distance(transform.position, targetPosition) > reachedDistance) 
            {
                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;

                if (Vector3.Distance(transform.position, targetPosition) < reachedDistance)
                {
                    transform.position = targetPosition;  // Snap to the exact position
                }
                yield return null;  // Wait for the next frame
            }
        }

        characterStateManager.ChangeState(CharacterStateManager.CharacterState.Idle);
        animator.SetTrigger("Idle");
    }
    #endregion

    #region Buff Handlers
    // Apply a buff based on its target scope
    public void ApplyBuff(BuffData buffData)
    {
        // For Healing
        if (buffData.statType == BuffData.StatType.Health)
        {
            currentHealth += (int)(characterData.baseHealth * buffData.amount);
            currentHealth = Mathf.Min(characterData.baseHealth, currentHealth);
            healthbar.UpdateValues(currentHealth, characterData.baseHealth);
            StartCoroutine(ShowHealingEffect());
        }
        else {
            activeBuffs.Add(buffData);
            buffTimers[buffData] = buffData.duration;
            ModifyStat(buffData.statType, buffData.amount);
        }
    }

    // Method to remove a buff and revert stat changes
    private void RemoveBuff(BuffData buffData)
    {
        ModifyStat(buffData.statType, -buffData.amount);  // Revert stat modification
    }

    // Helper method to modify the appropriate stat
    private void ModifyStat(BuffData.StatType statType, float amount)
    {
        switch (statType)
        {
            case BuffData.StatType.Attack:
                currentAttackPower += (int)(characterData.baseAttackPower * amount);
                break;
            case BuffData.StatType.Defense:
                currentDefense += (int)(characterData.baseDefense * amount);
                break;
            case BuffData.StatType.Speed:
                currentSpeed += amount;
                animator.speed = currentSpeed;
                break;
        }
    }

    // Method to show the VFX of healing
    private IEnumerator ShowHealingEffect()
    {
        healingAnimation.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        healingAnimation.SetActive(false);
    }
    #endregion

    #region Ability Functions
    public void ExecuteAbility(CharacterData.AttackType attackType, GameObject pfAbility, GameObject target, float targetOffsetY)
    {
        switch (attackType)
        {
            case CharacterData.AttackType.instant:
            // For instant attacks, destroy the object after instantiation
            SpawnAtTarget(pfAbility, target.transform.position + new Vector3(0, targetOffsetY, 0), true);
            break;
            
        case CharacterData.AttackType.cast:
            // For cast attacks, let the object handle its own destruction
            SpawnAtTarget(pfAbility, target.transform.position + new Vector3(0, targetOffsetY, 0), false);
            break;
                
            case CharacterData.AttackType.projectile:
                SpawnProjectile(pfAbility, target);
                break;
                
            case CharacterData.AttackType.selfBuff:
                ApplyBuff(pfAbility, new List<Transform> { transform });
                break;
                
            case CharacterData.AttackType.teamBuff:
                ApplyTeamBuff(pfAbility);
                break;

            case CharacterData.AttackType.center:
                SpawnAtTarget(pfAbility, new Vector3(0, 0), false);
                break;
                
            default:
                Debug.LogWarning("Unknown attack type");
                break;
        }
    }

    private void SpawnAtTarget(GameObject prefab, Vector3 position, bool destroyExternally)
    {
        GameObject spawnedObject = Instantiate(prefab, position, Quaternion.identity);
    
        if (destroyExternally)
        {
            Destroy(spawnedObject, 1.0f);  // Destroy after 1 second
        }
    }

    private void SpawnProjectile(GameObject projectilePrefab, GameObject target)
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        projectile.GetComponent<ProjectileHandler>().Setup(target.transform);
    }

    private void ApplyBuff(GameObject buffPrefab, List<Transform> targets)
    {
        GameObject buff = Instantiate(buffPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        buff.GetComponent<BuffHandler>().CastBuff(targets);
        Destroy(buff, 2.0f);
    }

    private void ApplyTeamBuff(GameObject buffPrefab)
    {
        GameObject buff = Instantiate(buffPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        List<Transform> teamTargets = new List<Transform>();

        foreach (GameObject character in BattleManager.instance.GetTeam())
        {
            if (character.GetComponent<CharacterStateManager>().currentState != CharacterStateManager.CharacterState.Dead)
            {
                teamTargets.Add(character.transform);
                if (character != this)
                {
                    SpawnAtTarget(buffPrefab, character.transform.position + new Vector3(0, 1f, 0), true);
                }
            }
        }

        buff.GetComponent<BuffHandler>().CastBuff(teamTargets);
        Destroy(buff, 2.0f);
    }
    #endregion
    public void TakeDamage(int attackValue)
    {
        if (!IsDead() && !InBurst())
        {           
            float calculatedDamage = DamageCalculator.CalculatePlayerDamage(attackValue, currentDefense);
            currentHealth = Mathf.Max(currentHealth - (int)calculatedDamage, 0);
            healthbar.UpdateValues(currentHealth, characterData.baseHealth);

            if (currentHealth == 0)
            {
                Death();
            }
            else
            {
                StartCoroutine(RedFlash());
            }
        }
    }
    
    public void GainEnergy(float amount)
    {
        currentEnergy = Mathf.Min(currentEnergy + amount, currentMaxEnergy);
        burstButton.UpdateSlider(currentEnergy / currentMaxEnergy);
    }

    IEnumerator RedFlash()
    {
        characterSpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        characterSpriteRenderer.color = Color.white;
    }

    public bool IsDead()
    {
        return characterStateManager.currentState == CharacterStateManager.CharacterState.Dead;
    }

    public bool InBurst()
    {
        return characterStateManager.currentState == CharacterStateManager.CharacterState.Burst;
    }

    public void Death()
    {
        StopAllCoroutines();
        animator.SetTrigger("Death");
        characterSpriteRenderer.color = Color.grey;
        characterStateManager.ChangeState(CharacterStateManager.CharacterState.Dead);
    }

    #region Wave Indicator
    public void StartWave()
    {
        waveInProgress = true;
    }

    public void EndWave()
    {
        waveInProgress = false;
    }
    #endregion
}
