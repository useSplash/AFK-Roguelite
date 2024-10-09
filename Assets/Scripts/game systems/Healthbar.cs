using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    public SpriteRenderer healthbar;
    public TextMeshPro textMesh;

    public void UpdateValues(float currentValue, float maxValue)
    {
        var tempTransform = healthbar.transform.localScale;
        tempTransform.x = Mathf.Clamp(currentValue/maxValue, 0, 1);

        healthbar.transform.localScale = tempTransform;

        textMesh.text = currentValue.ToString() + "/" + maxValue.ToString();
    }
}
