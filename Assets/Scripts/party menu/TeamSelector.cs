using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelector : MonoBehaviour
{
    public static TeamSelector instance { get; private set; }

    public CharacterData[] characterSlots = new CharacterData[3]; // Array to hold character data for each slot
    public SlotSelector[] slots = new SlotSelector[3];

    private int selectedSlotIndex = -1; // Index of the currently selected slot


    private void Start()
    {
        instance = this;
        ClearSlots();
        SelectNewSlot(0);
    }
    
    // CHARACTER SELECTOR
    public void SelectCharacter(CharacterData characterData)
    {
        if (characterData == null) return;
        // Implement logic to add the character to the team, or update UI, etc.
        Debug.Log("Character selected: " + characterData.characterName);
        slots[selectedSlotIndex].characterSprite.sprite = characterData.characterSprite;
    }

    // SLOT SELECTOR
    public void SelectNewSlot(int index)
    {
        if (selectedSlotIndex == index) return;
        if (selectedSlotIndex != -1) 
        {
            slots[selectedSlotIndex].HideSelector();
        }
        selectedSlotIndex = index;
        slots[selectedSlotIndex].ShowSelector();
    }

    public void ClearSlots()
    {
        foreach (SlotSelector slotSelector in slots)
        {
            slotSelector.characterSprite.sprite = null;
            slotSelector.HideSelector();
        }
    }
}
