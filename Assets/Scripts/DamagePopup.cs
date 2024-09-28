using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount, bool isCrit)
    {
        textMesh.SetText(damageAmount.ToString());
        if (isCrit)
        {
            textMesh.color = Color.red;
            textMesh.fontSize = 8;
        }
        Destroy(gameObject, 0.7f);
    }

    private void Update()
    {
        float moveSpeedY = 2f;
        transform.position += new Vector3(0, moveSpeedY) * Time.deltaTime;
    }
}
