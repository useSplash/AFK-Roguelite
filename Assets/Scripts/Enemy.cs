using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public BloodSplatRandomizer bloodSplatFX;
    public GameObject damageText;

    public void TakeDamage(){
        bloodSplatFX.PlayRandomSplat();
        DamagePopup damagePopup = Instantiate(damageText, 
                                              transform.position + new Vector3(0, 1), 
                                              Quaternion.identity)
                                                    .GetComponent<DamagePopup>();        
        damagePopup.Setup(99);
        StartCoroutine(RedFlash());
    }

    IEnumerator RedFlash(){
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = Color.white;
    }
}
