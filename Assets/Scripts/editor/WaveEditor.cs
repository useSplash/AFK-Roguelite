using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Wave))]
public class WaveEditor : Editor
{
    SerializedProperty spawnInstructions;

    private void OnEnable()
    {
        // Fetch the spawn instructions property
        spawnInstructions = serializedObject.FindProperty("spawnInstructions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Loop through spawn instructions and label them as Spawn 1, Spawn 2, etc.
        for (int i = 0; i < spawnInstructions.arraySize; i++)
        {
            SerializedProperty instruction = spawnInstructions.GetArrayElementAtIndex(i);

            // Display custom label for each spawn instruction
            EditorGUILayout.PropertyField(instruction, new GUIContent("Spawn " + (i + 1)));
        }

        if (GUILayout.Button("Add Spawn Instruction"))
        {
            spawnInstructions.InsertArrayElementAtIndex(spawnInstructions.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }
}