using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Moving,
        Ready,
        Attacking,
    }
    public EnemyState currentState;
    
    public void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }
}
