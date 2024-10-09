using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerEditor : Editor
{
    SerializedProperty waves;

    private void OnEnable()
    {
        // Fetch the Wave property
        waves = serializedObject.FindProperty("waves");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < waves.arraySize; i++)
        {
            SerializedProperty wave = waves.GetArrayElementAtIndex(i);
            
            // Display custom label for each wave
            EditorGUILayout.PropertyField(wave, new GUIContent("Wave " + (i + 1)));
        }

        if (GUILayout.Button("Add New Wave"))
        {
            waves.InsertArrayElementAtIndex(waves.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }
}