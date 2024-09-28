using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatRandomizer : MonoBehaviour
{
    Animator animator;
    void Awake(){
        animator = GetComponent<Animator>();
    }

    public void PlayRandomSplat(){
        int randomInt = Random.Range(1,3);
        animator.Play("bloodsplat_vfx"+randomInt, 0, 0);
    }
}
