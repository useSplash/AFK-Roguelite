using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<Wave> waves = new List<Wave>();
    public float timeBetweenWaves = 10f;

    private int currentWaveIndex = 0;
    private float waveTimer = 0f;
    private bool waveInProgress = false;

    private bool allEnemiesSpawned = false; // Track if all enemies have been spawned

    private void Update()
    {
        // If a wave is in progress, check for additional conditions
        if (waveInProgress)
        {
            // If all enemies are spawned and there are no more active enemies, the wave is complete
            if (allEnemiesSpawned && BattleManager.instance.GetEnemies().Length == 0)
            {
                waveInProgress = false;
                currentWaveIndex++;
                waveTimer = 0f; // Reset wave timer for the next wave
            }
            return;
        }

        if (currentWaveIndex < waves.Count && !waveInProgress)
        {
            waveTimer += Time.deltaTime;

            // If enough time has passed and all conditions are met, start the next wave
            if (waveTimer >= timeBetweenWaves)
            {
                waveTimer = 0f;
                StartWave(waves[currentWaveIndex]);
            }
        }
    }

    void StartWave(Wave wave)
    {
        waveInProgress = true;
        allEnemiesSpawned = false;  // Reset spawn flag
        foreach (GameObject characterObject in BattleManager.instance.GetTeam())
        {
            characterObject.GetComponent<Character>().StartWave();
        }
        Debug.Log("Start Wave " + (currentWaveIndex + 1).ToString());
        StartCoroutine(SpawnEnemiesInWave(wave));
    }

    IEnumerator SpawnEnemiesInWave(Wave wave)
    {
        foreach (var instruction in wave.spawnInstructions)
        {
            for (int i = 0; i < instruction.amount; i++)
            {
                SpawnEnemy(instruction.enemyData);
                yield return new WaitForSeconds(instruction.interval); // Delay between spawns
            }
        }
        
        // All enemies have been spawned
        allEnemiesSpawned = true;
    }

    private void SpawnEnemy(EnemyData enemyData)
    {
        // Get an enemy from the pool
        GameObject enemy = EnemyPooler.instance.GetPooledEnemy();
        if (enemy == null)
        {
            return;
        }

        // Reinitialize enemy data, reset health, etc. (if necessary)
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.enemyData = enemyData;
        enemyScript.InitializeEnemy();

        // Get a random spawn point
        float randomFloatY = UnityEngine.Random.Range(-1.0f, 1.0f);
        Vector3 spawnPoint = new Vector3(10f, randomFloatY - 2.5f);

        // Place the enemy at the spawn point
        enemy.transform.position = spawnPoint;
    }
}
