using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    private Transform target; 
    private Vector3 moveDir;
    private float targetOffsetY;

    [SerializeField] private float speed = 10f;  // Default speed
    [SerializeField] private bool isHoming = false;  // Default homing behavior
    [SerializeField] private bool destroyOnImpact = true;  // Default impact behavior
    [SerializeField] private GameObject destroyVFX;

    public void Setup(Transform target)
    {
        this.target = target;
        if (target.GetComponent<Collider2D>() != null)
        {
            targetOffsetY = target.GetComponent<Collider2D>().bounds.size.y/3 + 0.3f;
        }

        moveDir = (target.position 
                    + new Vector3(0, targetOffsetY, 0)
                    - transform.position).normalized;

        // Automatically destroy non-homing projectiles after 2 seconds
        if (!isHoming)
        {
            Destroy(gameObject, 5.0f);
        }
    }

    void Update()
    {
        if (isHoming)
        {
            if (target == null)
            {
                moveDir = (target.position 
                        + new Vector3(0, targetOffsetY, 0)
                        - transform.position).normalized;
                
                float destroySelfDistance = 0.2f;
                if (Vector3.Distance(target.position + new Vector3(0, targetOffsetY, 0) 
                                    ,transform.position) 
                                        < destroySelfDistance)
                {
                    Destroy(gameObject);
                }
            }
            else {
                Destroy(gameObject, 5.0f);
                isHoming = false;
            }
        }

        // Move projectile
        transform.position += moveDir * speed * Time.deltaTime;
    }
    
    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.tag == "Enemy" && destroyOnImpact)
        {
            Destroy(gameObject, 0.1f);
        }
    }

    private void OnDestroy(){
        if (destroyVFX != null){
            Damage projectileDamage = GetComponent<Damage>();
            Damage impactDamage = destroyVFX.GetComponent<Damage>();
            if (projectileDamage != null && impactDamage != null)
            {
                impactDamage.Setup(projectileDamage.sourceAttackDamage, 
                                   projectileDamage.sourceDamageMultiplier,
                                   projectileDamage.sourceCriticalChance,
                                   projectileDamage.sourceCriticalMultiplier);
            }
            Destroy(Instantiate(destroyVFX, 
                                transform.position, 
                                Quaternion.identity), 0.5f);
        }
    }
    
}
