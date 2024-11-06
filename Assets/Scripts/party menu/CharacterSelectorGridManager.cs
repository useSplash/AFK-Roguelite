using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterSelectorGridManager : MonoBehaviour
{
    public List<CharacterData> characterList; // List of all character data to display
    public GameObject characterPortraitPrefab; // Prefab for character portrait buttons
    public GameObject clearButtonPrefab; // Prefab for the clear button
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

        // Loop through each character in the list and create a button for it
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

            // Initialize the button with character data
            CharacterSelectButton button = portrait.GetComponent<CharacterSelectButton>();
            if (button != null)
            {
                button.Initialize(characterList[i]);
            }
        }

        // Add a clear button at the end
        AddClearButton(startX, startY);
    }

    void AddClearButton(float startX, float startY)
    {
        int totalItems = characterList.Count;
        int row = totalItems / columns;
        int column = totalItems % columns;

        // Calculate position for the clear button
        float xPos = startX + column * (cellWidth + spacing);
        float yPos = startY - row * (cellHeight + spacing);
        Vector3 position = new Vector3(xPos, yPos, 0);

        // Instantiate and set up the clear button
        GameObject clearButton = Instantiate(clearButtonPrefab, gridContainer);
        clearButton.GetComponent<RectTransform>().localPosition = position;

        // Assign the clear functionality
        clearButton.GetComponent<Button>().onClick.AddListener(() => TeamSelector.instance.ClearSelectedSlot());
    }
}