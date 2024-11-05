using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CharacterSelectorGridManager : MonoBehaviour
{
    public List<CharacterData> characterList; // List of all character data to display
    public GameObject characterPortraitPrefab; // Prefab for character portrait buttons
    public RectTransform gridContainer; // Container for the grid layout
    public int columns = 4; // Number of columns
    public float cellWidth = 100f; // Width of each cell
    public float cellHeight = 100f; // Height of each cell
    public float spacing = 10f; // Spacing between cells

    void Start()
    {
        PopulateCustomGrid();
    }

    void PopulateCustomGrid()
    {
        float startX = -gridContainer.rect.width / 2 + cellWidth / 2;
        float startY = gridContainer.rect.height / 2 - cellHeight / 2;

        for (int i = 0; i < characterList.Count; i++)
        {
            // Calculate row and column
            int row = i / columns;
            int column = i % columns;

            // Calculate position
            float xPos = startX + column * (cellWidth + spacing);
            float yPos = startY - row * (cellHeight + spacing);
            Vector3 position = new Vector3(xPos, yPos, 0);

            // Instantiate and set up the character portrait button
            GameObject portrait = Instantiate(characterPortraitPrefab, gridContainer);
            portrait.GetComponent<RectTransform>().localPosition = position;

            // Initialize the button with character data and team selector reference
            CharacterSelectButton button = portrait.GetComponent<CharacterSelectButton>();
            if (button != null)
            {
                button.Initialize(characterList[i]);
            }
        }
    }
}