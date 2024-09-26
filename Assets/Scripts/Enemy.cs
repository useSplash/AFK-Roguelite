using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public BloodSplatRandomizer bloodSplatFX;

    public void TakeDamage(){
        bloodSplatFX.PlayRandomSplat();
        StartCoroutine(RedFlash());
    }

    IEnumerator RedFlash(){
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = Color.white;
    }
}
