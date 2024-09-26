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
        selfBuff,
        teamBuff,
    }
    public string characterName;
    public Sprite characterSprite;

    [Header("Stats")]
    public int baseHealth;
    public int baseAttackPower;
    public int baseDefense;

    [Header("Basic Attack")]
    public float basicAttackWindupDuration;
    public float basicAttackRecoilDuration;
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
    public AttackType specialAbilityType;
    public GameObject pfSpecialAbility;

    [Header("Special Ability Animation Trigger")]
    public string triggerSpecialAbilityWindup;
    public string triggerSpecialAbilityRelease;
    public string triggerSpecialAbilityReel;
}
