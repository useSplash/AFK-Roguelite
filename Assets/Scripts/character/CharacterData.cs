using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character Data")]
public class CharacterData : ScriptableObject
{
    public enum AttackType{
        instant, // like a slash
        projectile, // like an arrow
        cast, // like a spell
        selfBuff, // increasing own stats
        teamBuff, // increasing team stats
        center, // screen wide fixed position
    }
    public string characterName;
    public Sprite characterSprite;

    [Header("Stats")]
    public int baseHealth;
    public int baseAttackPower;
    public int baseDefense;
    public float baseCritChance;
    public float baseCritDamage;

    [Header("Basic Attack")]
    public float basicAttackWindupDuration;
    public float basicAttackRecoilDuration;
    public float basicAttackDamageMultiplier;
    public AttackType basicAttackType;
    public GameObject pfBasicAttack;

    [Header("Basic Attack Animation Trigger")]
    public string triggerBasicAttackWindup;
    public string triggerBasicAttackRelease;
    public string triggerBasicAttackReel;

    [Header("Special Ability")]
    public float specialAbilityWindupDuration;
    public float specialAbilityRecoilDuration;
    public float specialAbilityCooldown;
    public float specialAbilityDamageMultiplier;
    public AttackType specialAbilityType;
    public GameObject pfSpecialAbility;

    [Header("Special Ability Animation Trigger")]
    public string triggerSpecialAbilityWindup;
    public string triggerSpecialAbilityRelease;
    public string triggerSpecialAbilityReel;

    [Header("Burst Ability")]
    public string burstAbilityName;
    public float burstAbilityWindupDuration;
    public float burstAbilityRecoilDuration;
    public float burstAbilityDamageMultiplier;
    public AttackType burstAbilityType;
    public GameObject pfBurstAbility;
    public bool travelToCenter;

    [Header("Burst Ability Animation Trigger")]
    public string triggerBurstAbilityWindup;
    public string triggerBurstAbilityRelease;
    public string triggerBurstAbilityReel;
}
