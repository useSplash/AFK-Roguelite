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

    // Character special attack variables
    private float currentSpecialAbilityWindupDuration;
    private float currentSpecialAbilityRecoilDuration;
    private float currentSpecialAbilityCooldown;
    #endregion
    
    #region Buff Information
    private List<BuffData> activeBuffs = new List<BuffData>();
    private Dictionary<BuffData, float> buffTimers = new Dictionary<BuffData, float>();
    #endregion

    private GameObject target;
    private float specialAbilityCooldownTimer;

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
            buffTimers[buff] -= Time.deltaTime;

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
            if (activeBuffs.Count >= 2){
                // Case when 2 or more buffs are active
                emission.rateOverTime = new ParticleSystem.MinMaxCurve(50);
                switch (activeBuffs[1].statType){
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
            }
            else 
            {
                buffColor2 = buffColor1;
            }
            main.startColor = new ParticleSystem.MinMaxGradient(buffColor1, buffColor2);
        }
        #endregion

        // Update special ability timer
        specialAbilityCooldownTimer -= Time.deltaTime;
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

        // Perform Attack
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
        switch (characterData.basicAttackType){
            case CharacterData.AttackType.instant:
                Destroy(Instantiate(pfAttack, 
                                    target.transform.position + new Vector3(0, targetOffsetY, 0), 
                                    Quaternion.identity), 
                                        1.0f);
                break;
            case CharacterData.AttackType.projectile:
                GameObject projectile = Instantiate(pfAttack, 
                                        transform.position + new Vector3(0, 1f, 0), // Position starts at the base 
                                        Quaternion.identity);
                projectile.GetComponent<ProjectileHandler>().Setup(target.transform);
                break;
            case CharacterData.AttackType.cast:
                Instantiate(pfAttack, 
                            target.transform.position + new Vector3(0, targetOffsetY, 0), 
                            Quaternion.identity);
                break;
            default:
                break;
        }
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
        if (BattleManager.instance.GetClosestEnemy(transform.position) != null)
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
        switch (characterData.specialAbilityType){
            case CharacterData.AttackType.instant:
                Destroy(Instantiate(pfAbility, 
                                    target.transform.position + new Vector3(0, targetOffsetY, 0), 
                                    Quaternion.identity), 
                                        1.0f);
                break;
            case CharacterData.AttackType.projectile:
                GameObject projectile = Instantiate(pfAbility, 
                                        transform.position + new Vector3(0, 1f, 0), // Position starts at the base 
                                        Quaternion.identity);
                projectile.GetComponent<ProjectileHandler>().Setup(target.transform);
                break;
            case CharacterData.AttackType.cast:
                Instantiate(pfAbility, 
                            target.transform.position + new Vector3(0, targetOffsetY, 0), 
                            Quaternion.identity);
                break;
            case CharacterData.AttackType.selfBuff:
                GameObject buff = Instantiate(characterData.pfSpecialAbility, 
                                    transform.position + new Vector3(0, 1f, 0), // Position starts at the base 
                                    Quaternion.identity);
                List<Transform> buffTargets = new List<Transform>
                {
                    transform
                };
                buff.GetComponent<BuffHandler>().CastBuff(buffTargets);
                Destroy(buff, 2.0f);
                break;
            case CharacterData.AttackType.teamBuff:
                buff = Instantiate(pfAbility, 
                                   transform.position + new Vector3(0, 1f, 0), // Position starts at the base 
                                   Quaternion.identity);
                buffTargets = new List<Transform>();
                foreach (GameObject character in BattleManager.instance.GetTeam())
                {
                    if (character.GetComponent<CharacterStateManager>().currentState != CharacterStateManager.CharacterState.Dead)
                    {
                        buffTargets.Add(character.transform);
                        if (character != this){
                            Destroy(Instantiate(characterData.pfSpecialAbility, 
                                    character.transform.position + new Vector3(0, 1f, 0), // Position starts at the base 
                                    Quaternion.identity), 2.0f);
                        }
                    }
                }
                buff.GetComponent<BuffHandler>().CastBuff(buffTargets);
                Destroy(buff, 2.0f);
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(currentSpecialAbilityRecoilDuration / currentSpeed);
        animator.SetTrigger(characterData.triggerSpecialAbilityReel);
        yield return new WaitForSeconds(0.5f / currentSpeed);
        specialAbilityCooldownTimer = currentSpecialAbilityCooldown;
        characterStateManager.ChangeState(CharacterStateManager.CharacterState.Idle);
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

    public void TakeDamage(int attackValue)
    {
        if (characterStateManager.currentState != CharacterStateManager.CharacterState.Dead)
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

    IEnumerator RedFlash()
    {
        characterSpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        characterSpriteRenderer.color = Color.white;
    }

    public void Death()
    {
        StopAllCoroutines();
        animator.SetTrigger("Death");
        characterSpriteRenderer.color = Color.grey;
        characterStateManager.ChangeState(CharacterStateManager.CharacterState.Dead);
    }
}
