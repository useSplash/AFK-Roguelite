using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateManager : MonoBehaviour
{
    public enum CharacterState 
    {
        Idle,
        Attacking,
        Stunned,
        Dead
    }
    public CharacterState currentState;

    public void ChangeState(CharacterState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }
}
