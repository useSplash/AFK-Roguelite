using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance { get; private set; }
    public GameObject characterPrefab; // Prefab with Character script attached
    public CharacterData[] teamList = new CharacterData[3];
    
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetupTeam();
    }

    public void SetupTeam()
    {
        for (int i = 0; i < teamList.Length; i++)
        {
            if (teamList[i] != null)
            {
                // Change position based on slot
                Vector3 characterPosition;
                switch (i){
                    case 0:
                        characterPosition = new Vector3(-2f, -2f);
                        break;
                    case 1:
                        characterPosition = new Vector3(-4.5f, -3f);
                        break;
                    case 2:
                        characterPosition = new Vector3(-6.5f, -2f);
                        break;
                    default:
                        characterPosition = Vector3.zero;
                        break;
                }

                SpawnCharacter(characterPosition, teamList[i]);
            }
        }
    }

    public void SpawnCharacter(Vector3 spawnPosition, CharacterData characterData)
    {
        GameObject newCharacter = Instantiate(characterPrefab, spawnPosition, Quaternion.identity);

        // Assign the appropriate CharacterData based on the index or some other logic
        Character character = newCharacter.GetComponent<Character>();
        character.characterData = characterData;

        // Initialize the character with the correct data
        character.InitializeCharacter();
    }

    public GameObject[] GetEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy");
    }

    public GameObject[] GetTeam()
    {
        return GameObject.FindGameObjectsWithTag("Player");
    }
    
    public GameObject GetClosestEnemy(Vector3 position)
    {
        GameObject[] enemies = GetEnemies();
        if (enemies == null || enemies.Length == 0) return null;
        
        GameObject closestEnemy = enemies[0];
        
        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(position, closestEnemy.transform.position) >
                Vector3.Distance(position, enemy.transform.position))
            {
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}
