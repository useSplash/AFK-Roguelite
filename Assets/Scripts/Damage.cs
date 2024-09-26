using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Enemy"){
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy) {
                enemy.TakeDamage();
            }
        }
    }
}
