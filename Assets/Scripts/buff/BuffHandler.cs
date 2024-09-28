using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffHandler : MonoBehaviour
{
    public List<BuffData> buffDataList;

    public void CastBuff(List<Transform> targets)
    {
        foreach (Transform character in targets)
        {
            foreach (BuffData buff in buffDataList)
            {
                character.GetComponent<Character>().ApplyBuff(buff);
            }
        }
    }
}
