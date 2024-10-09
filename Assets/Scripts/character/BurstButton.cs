using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BurstButton : MonoBehaviour
{
    Character character;
    public Slider gauge;
    public Image charactePortrait;

    public void InitializeBurstButton(Character _character)
    {
        if (_character == null)
        {
            charactePortrait.sprite = null;
            UpdateGauge(0);
            return;
        }
        character = _character;
        charactePortrait.sprite = character.characterData.characterSprite;
        UpdateGauge(0);
    }

    public void UpdateGauge(float value)
    {
        gauge.value = value;
    }

    public void ActivateBurst()
    {
        if (!character.IsDead() && character != null)
        {
            character.ActivateBurst();
        }
    }
}
