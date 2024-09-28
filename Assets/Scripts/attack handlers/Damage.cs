using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public int sourceAttackDamage;
    public float sourceDamageMultiplier;
    public float sourceCriticalChance;
    public float sourceCriticalMultiplier;

    public void Setup(int attackDamage, float damageMultiplier, float criticalChance, float criticalMultiplier){
        sourceAttackDamage = attackDamage;
        sourceDamageMultiplier = damageMultiplier;
        sourceCriticalChance = criticalChance;
        sourceCriticalMultiplier = criticalMultiplier;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Enemy"){
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy) {
                enemy.TakeDamage(DamageCalculator.CalculateDamage(sourceAttackDamage, sourceDamageMultiplier, sourceCriticalChance, sourceCriticalMultiplier));
            }
        }
    }
}
