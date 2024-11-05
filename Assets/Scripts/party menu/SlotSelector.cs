using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotSelector : MonoBehaviour
{
    public int slotIndex; // Index of the slot this selector is for
    public GameObject selector;
    public SpriteRenderer characterSprite;

    public void OnSlotButtonClicked()
    {
        TeamSelector.instance.SelectNewSlot(slotIndex); // Inform the TeamSelector which slot was clicked
    }

    public void HideSelector()
    {
        selector.SetActive(false);
    }

    public void ShowSelector()
    {
        selector.SetActive(true);
    }
}
