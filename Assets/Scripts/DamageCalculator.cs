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

    public static int CalculatePlayerDamage(int enemyAttack, int playerDefense)
    {
        // Defense reduces damage by a percentage, capped to prevent zero damage
        float defenseFactor = 1 - (playerDefense / (playerDefense + 100f)); // Defense gets weaker with higher values
        int damage = Mathf.CeilToInt(enemyAttack * defenseFactor);
        return Mathf.Max(damage, 1); // Ensure at least 1 damage
    }
}
