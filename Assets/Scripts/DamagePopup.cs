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

    public void Setup(int damageAmount)
    {
        textMesh.SetText(damageAmount.ToString());
        Destroy(gameObject, 0.7f);
    }

    private void Update()
    {
        float moveSpeedY = 2f;
        transform.position += new Vector3(0, moveSpeedY) * Time.deltaTime;
    }
}
