using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public enum AttackType{
        instant, // like a slash
        projectile, // like an arrow
    }
    public string enemyName;
    public Sprite enemySprite;

    [Header("Stats")]
    public int baseHealth;
    public int baseAttack;
    public float baseAttackRange;

    [Header("Movement")]
    public float baseStepDistance;
    public float baseStepDuration;
    public float baseStepInterval;
    
    [Header("Attack")]
    public float attackWindupDuration;
    public float attackRecoilDuration;
    public float attackCooldown;
    public AttackType attackType;
    

    [Header("Attack Animation Trigger")]
    public string triggerAttackWindup;
    public string triggerAttackRelease;
    public string triggerAttackReel;
}
