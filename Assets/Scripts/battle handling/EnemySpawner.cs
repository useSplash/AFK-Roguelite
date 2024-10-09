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

    private int activeEnemies = 0; // Track the number of active enemies
    private int totalEnemiesSpawned = 0; // Track total number of enemies spawned


    private void Update()
    {
        // If a wave is in progress, check for additional conditions
        if (waveInProgress)
        {
            // If there are no more active enemies, the wave is complete
            if (activeEnemies <= 0)
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
        activeEnemies = 0; // Reset active enemies for the new wave
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
                activeEnemies++; // Increment the active enemy count
                totalEnemiesSpawned++; // Increment the total enemies spawned count
                Debug.Log("Total Enemy Spawned: " + totalEnemiesSpawned);
                yield return new WaitForSeconds(instruction.interval); // Delay between spawns
            }
        }
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

        // Subscribe to the enemy's death event to decrement activeEnemies count
        enemyScript.OnDeath += HandleEnemyDeath;

        // Get a random spawn point
        float randomFloatY = Random.Range(-1.0f, 1.0f);
        Vector3 spawnPoint = new Vector3(10f, randomFloatY - 2.5f);

        // Place the enemy at the spawn point
        enemy.transform.position = spawnPoint;
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        activeEnemies--; // Decrement the count when an enemy dies
        Debug.Log("Enemy defeated. Active enemies left: " + activeEnemies);

        // Unsubscribe from the death event to prevent multiple triggers
        enemy.OnDeath -= HandleEnemyDeath;
    }
}
