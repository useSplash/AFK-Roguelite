using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectButton : MonoBehaviour
{
    public Image characterImage; // UI Image component for the character portrait
    public TextMeshProUGUI characterNameText; // TextMeshPro component for the character name
    public CharacterData characterData; // The character data for this button

    public Sprite defaultImage;

    // Initialize the button with character data and team selector
    public void Initialize(CharacterData data)
    {
        characterData = data;

        if (characterData != null)
        {
            characterImage.sprite = characterData.characterPortrait;
            characterNameText.text = characterData.characterName;
        }
        else
        {
            characterImage.sprite = defaultImage; // Optionally set a default sprite
            characterNameText.text = "???";
        }
    }

    // Called when the button is clicked
    public void OnButtonClicked()
    {
        if (TeamSelector.instance != null && characterData != null)
        {
            TeamSelector.instance.SelectCharacter(characterData); // Pass character data to the team selector
        }
    }
}
