using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
            targetOffsetY = target.GetComponent<Collider2D>().bounds.size.y/3;
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
            moveDir = (target.position 
                    + new Vector3(0, targetOffsetY, 0)
                    - transform.position).normalized;
        }

        // Destroy projectile on impact if enabled
        if (destroyOnImpact)
        {
            float destroySelfDistance = 0.2f;
            if (Vector3.Distance(target.position + new Vector3(0, targetOffsetY, 0) 
                                ,transform.position) 
                                    < destroySelfDistance)
            {
                if (destroyVFX != null){
                    Destroy(Instantiate(destroyVFX, 
                                        transform.position, 
                                        Quaternion.identity), 0.5f);
                }
                Destroy(gameObject);
            }
        }

        // Move projectile
        transform.position += moveDir * speed * Time.deltaTime;
    }
}
