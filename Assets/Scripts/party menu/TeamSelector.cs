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

        // Check if character is already in another slot
        int existingSlotIndex = FindCharacterInSlots(characterData);

        if (existingSlotIndex == -1)
        {
            // Character is not in any slot, so assign it to the selected slot
            characterSlots[selectedSlotIndex] = characterData;
        }
        else
        {
            if (characterSlots[selectedSlotIndex] == null)
            {
                // If the selected slot is empty, move the character from its existing slot
                characterSlots[selectedSlotIndex] = characterData;
                characterSlots[existingSlotIndex] = null; // Clear the previous slot
            }
            else
            {
                // If both slots are occupied, swap the characters
                CharacterData temp = characterSlots[selectedSlotIndex];
                characterSlots[selectedSlotIndex] = characterSlots[existingSlotIndex];
                characterSlots[existingSlotIndex] = temp;
            }
        }

        // Update the slot sprites and UI
        UpdateAllSlotsDisplay();
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

    // Finds the index of a character in the team, or -1 if not found
    private int FindCharacterInSlots(CharacterData characterData)
    {
        for (int i = 0; i < characterSlots.Length; i++)
        {
            if (characterSlots[i] == characterData)
            {
                return i;
            }
        }
        return -1;
    }

    // Clears all slots
    public void ClearSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            characterSlots[i] = null;
            slots[i].characterSprite.sprite = null;
            slots[i].HideSelector();
        }
    }

    // Update all slot displays
    private void UpdateAllSlotsDisplay()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (characterSlots[i] != null)
            {
                slots[i].characterSprite.sprite = characterSlots[i].characterSprite;
            }
            else
            {
                slots[i].characterSprite.sprite = null; // Clear sprite if no character is assigned
            }
        }
    }

    // Clears the character in the currently selected slot
    public void ClearSelectedSlot()
    {
        if (selectedSlotIndex >= 0 && selectedSlotIndex < characterSlots.Length)
        {
            characterSlots[selectedSlotIndex] = null;
            slots[selectedSlotIndex].characterSprite.sprite = null;
            Debug.Log("Cleared character from selected slot: " + selectedSlotIndex);
        }
    }
}
