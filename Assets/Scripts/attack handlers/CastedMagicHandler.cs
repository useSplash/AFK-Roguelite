using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastedMagicHandler : MonoBehaviour
{
    public void DestroySpell()
    {
        Destroy(gameObject);
    }

    public void ShakeScreen()
    {
        CameraShake.instance.HeavyImpactShake();
    }
}
