using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttackHandler : MonoBehaviour
{
    public void DestroyAttack()
    {
        Destroy(gameObject);
    }

    public void TimeStop()
    {
        TimeStopHandler.instance.StopTimeForImpact(0.1f);
    }

    public void ShakeScreen()
    {
        CameraShake.instance.HeavyImpactShake();
    }
}
