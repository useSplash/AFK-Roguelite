using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatRandomizer : MonoBehaviour
{
    Animator animator;
    SpriteRenderer spriteRenderer;
    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayRandomSplat()
    {
        int randomInt = Random.Range(1,3);
        animator.Play("bloodsplat_vfx"+randomInt, 0, 0);
    }

    public void Reset()
    {
        animator.Play("Idle");
        spriteRenderer.sprite = null;
    }
}
