using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int currentHealth = 5000;

    public SpriteRenderer spriteRenderer;
    public BloodSplatRandomizer bloodSplatFX;
    public GameObject damageText;
    public GameObject deathPoof;
    public bool isInvincible;

    public void TakeDamage((float damageAmount, bool isCrit) damageInfo){
        bloodSplatFX.PlayRandomSplat();
        DamagePopup damagePopup = Instantiate(damageText, 
                                              transform.position + new Vector3(0, 1), 
                                              Quaternion.identity)
                                                    .GetComponent<DamagePopup>();        
        damagePopup.Setup((int)damageInfo.damageAmount, damageInfo.isCrit);
        currentHealth -= (int)damageInfo.damageAmount;
        if (currentHealth <= 0 && !isInvincible)
        {
            Death();
        }
        StartCoroutine(RedFlash());
    }

    IEnumerator RedFlash(){
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = Color.white;
    }

    public void Death(){
        Destroy(gameObject);
    }

    private void OnDestroy(){
        Destroy(Instantiate(deathPoof,
                            transform.position + new Vector3(0, 1), 
                            Quaternion.identity), 1.0f);
    }
}
