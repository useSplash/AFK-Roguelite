using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Buff")]
public class BuffData : ScriptableObject
{
    public enum StatType 
    { 
        Attack, 
        Defense, 
        Speed, 
        Health 
    }
    public StatType statType;
    public float amount;
    public float duration;
}
