using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterData characterData;
    public ParticleSystem buffParticleSystem;
    public GameObject healingAnimation;

    #region Components
    Animator animator;
    CharacterStateManager characterStateManager;
    SpriteRenderer spriteRenderer;
    #endregion

    #region CharacterData
    // Character stats
    private int currentHealth;
    private int currentAttackPower;
    private int currentDefense;
    
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

    private void Awake(){
        animator = GetComponent<Animator>();
        characterStateManager = GetComponent<CharacterStateManager>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        healingAnimation.SetActive(false);
    }

    public void InitializeCharacter()
    {
        spriteRenderer.sprite = characterData.characterSprite;

        // Initialize stats from CharacterData
        currentHealth = characterData.baseHealth;
        currentAttackPower = characterData.baseAttackPower;
        currentDefense = characterData.baseDefense;

        currentBasicAttackWindupDuration = characterData.basicAttackWindupDuration;
        currentBasicAttackRecoilDuration = characterData.basicAttackRecoilDuration;

        currentSpecialAbilityWindupDuration = characterData.specialAbilityWindupDuration;
        currentSpecialAbilityRecoilDuration = characterData.specialAbilityRecoilDuration;
        currentSpecialAbilityCooldown = characterData.specialAbilityCooldown;

        // Set state to idle
        characterStateManager.ChangeState(CharacterStateManager.CharacterState.Idle);
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

        // Update special ability timer
        specialAbilityCooldownTimer -= Time.deltaTime;
        if (specialAbilityCooldownTimer < 0) specialAbilityCooldownTimer = 0;

        // Change behavior based on state
        switch (characterStateManager.currentState)
        {
            case CharacterStateManager.CharacterState.Idle:
                target = BattleManager.instance.GetClosestEnemy(transform.position);
                if (target != null){
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
    
    private IEnumerator BasicAttackWindup()
    {
        animator.SetTrigger(characterData.triggerBasicAttackWindup);
        yield return new WaitForSeconds(currentBasicAttackWindupDuration);
        StartCoroutine(BasicAttackRelease());
    }

    private IEnumerator BasicAttackRelease()
    {
        animator.SetTrigger(characterData.triggerBasicAttackRelease);
        float targetOffsetY = target.GetComponent<Collider2D>().bounds.size.y/3;
        switch (characterData.basicAttackType){
            case CharacterData.AttackType.instant:
                Destroy(Instantiate(characterData.pfBasicAttack, 
                                    target.transform.position + new Vector3(0, targetOffsetY, 0), 
                                    Quaternion.identity), 
                                        1.0f);
                break;
            case CharacterData.AttackType.projectile:
                GameObject projectile = Instantiate(characterData.pfBasicAttack, 
                                    transform.position + new Vector3(0, 1f, 0), // Position starts at the base 
                                    Quaternion.identity);
                projectile.GetComponent<ProjectileHandler>().Setup(target.transform);
                break;
            case CharacterData.AttackType.cast:
                Instantiate(characterData.pfBasicAttack, 
                                    target.transform.position + new Vector3(0, targetOffsetY, 0), 
                                    Quaternion.identity);
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(currentBasicAttackRecoilDuration);
        animator.SetTrigger(characterData.triggerBasicAttackReel);
        yield return new WaitForSeconds(0.5f);
        characterStateManager.ChangeState(CharacterStateManager.CharacterState.Idle);
    }

    private IEnumerator SpecialAbilityWindup()
    {
        animator.SetTrigger(characterData.triggerSpecialAbilityWindup);
        yield return new WaitForSeconds(currentSpecialAbilityWindupDuration);
        StartCoroutine(SpecialAbilityRelease());
    }

    private IEnumerator SpecialAbilityRelease()
    {
        animator.SetTrigger(characterData.triggerSpecialAbilityRelease);
        float targetOffsetY = target.GetComponent<Collider2D>().bounds.size.y/3;
        switch (characterData.specialAbilityType){
            case CharacterData.AttackType.instant:
                Destroy(Instantiate(characterData.pfSpecialAbility, 
                                    target.transform.position + new Vector3(0, targetOffsetY, 0), 
                                    Quaternion.identity), 
                                        1.0f);
                break;
            case CharacterData.AttackType.projectile:
                GameObject projectile = Instantiate(characterData.pfSpecialAbility, 
                                    transform.position + new Vector3(0, 1f, 0), // Position starts at the base 
                                    Quaternion.identity);
                projectile.GetComponent<ProjectileHandler>().Setup(target.transform);
                break;
            case CharacterData.AttackType.cast:
                Instantiate(characterData.pfSpecialAbility, 
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
                buff = Instantiate(characterData.pfSpecialAbility, 
                                    transform.position + new Vector3(0, 1f, 0), // Position starts at the base 
                                    Quaternion.identity);
                buffTargets = new List<Transform>();
                foreach (GameObject character in BattleManager.instance.GetTeam())
                {
                    buffTargets.Add(character.transform);
                    if (character != this){
                        Destroy(Instantiate(characterData.pfSpecialAbility, 
                                character.transform.position + new Vector3(0, 1f, 0), // Position starts at the base 
                                Quaternion.identity), 2.0f);
                    }
                }
                buff.GetComponent<BuffHandler>().CastBuff(buffTargets);
                Destroy(buff, 2.0f);
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(currentSpecialAbilityRecoilDuration);
        animator.SetTrigger(characterData.triggerSpecialAbilityReel);
        yield return new WaitForSeconds(0.5f);
        specialAbilityCooldownTimer = currentSpecialAbilityCooldown;
        characterStateManager.ChangeState(CharacterStateManager.CharacterState.Idle);
    }

    // Apply a buff based on its target scope
    public void ApplyBuff(BuffData buffData)
    {
        if (buffData.statType == BuffData.StatType.Health)
        {
            currentHealth += (int)(characterData.baseHealth * buffData.amount);
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
                currentDefense += (int)amount;
                break;
            case BuffData.StatType.Speed:
                // Modify speed if implemented
                break;
        }
    }

    private IEnumerator ShowHealingEffect()
    {
        healingAnimation.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        healingAnimation.SetActive(false);
    }
}
