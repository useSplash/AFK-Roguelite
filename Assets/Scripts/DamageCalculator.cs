using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageCalculator
{
    public static (int damage, bool isCrit) CalculateDamage(int sourceAttackPower, float baseAttackMultiplier, float criticalChance, float criticalMultiplier = 1.0f)
    {
        bool isCrit = false;
        float damage = sourceAttackPower * baseAttackMultiplier;
        
        // Roll for critical hit
        if (Random.value * 100 <= criticalChance)
        {
            damage *= criticalMultiplier;  // Apply critical hit multiplier
            isCrit = true;
        }

        // Add randomness
        float randomMultiplier = Random.Range(0.95f, 1.05f);
        damage *= randomMultiplier;

        // Ensure damage isn't negative
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(damage));

        return (finalDamage, isCrit); 
    }
}
