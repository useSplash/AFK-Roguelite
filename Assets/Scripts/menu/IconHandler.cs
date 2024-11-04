using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconHandler : MonoBehaviour
{
    [SerializeField] string buttonName;

    public void TestButton()
    {
        Debug.Log("Clickity Click: " + buttonName);
    }

}
