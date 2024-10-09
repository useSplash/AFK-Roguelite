using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Wave))]
public class WaveEditor : Editor
{
    SerializedProperty spawnInstructions;

    private void OnEnable()
    {
        // Fetch the spawnInstructions property
        spawnInstructions = serializedObject.FindProperty("spawnInstructions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Loop through each spawn instruction and label them as Spawn 1, 2, etc.
        for (int i = 0; i < spawnInstructions.arraySize; i++)
        {
            SerializedProperty instruction = spawnInstructions.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(instruction, new GUIContent("Spawn " + (i + 1)));
        }

        // Button to add more spawn instructions
        if (GUILayout.Button("Add Spawn Instruction"))
        {
            spawnInstructions.InsertArrayElementAtIndex(spawnInstructions.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }
}