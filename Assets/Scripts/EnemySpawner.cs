using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnInstruction
    {
        public EnemyData enemyData;
        public int amount;
        public float interval;
    }

    [System.Serializable]
    public class Wave
    {
        public List<SpawnInstruction> spawnInstructions;
    }

    public List<Wave> waves;
    public EnemyData enemyDataA;
    public EnemyData enemyDataB;
    public float spawnInterval = 5f; // Time interval between spawns
    private float spawnTimer;

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            int randomInt = Random.Range(1,3);
            if (randomInt == 1)
            {
                if (Random.Range(0, 100) < 50)
                {
                    SpawnEnemy(enemyDataB);
                }
                else
                {
                    SpawnEnemy(enemyDataA);
                }
            }
            else
            {
                for (int i = 0; i < randomInt; i++)
                {
                    SpawnEnemy(enemyDataA);
                }
            }
            spawnTimer = spawnInterval + Random.Range(-1,1);  // Reset the timer
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

        // Get a random spawn point
        float randomFloatY = Random.Range(-1.0f, 1.0f);
        Vector3 spawnPoint = new Vector3(10f, randomFloatY - 2.5f);


        // Place the enemy at the spawn point
        enemy.transform.position = spawnPoint;

        // Reinitialize enemy data, reset health, etc. (if necessary)
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.enemyData = enemyData;
        enemyScript.InitializeEnemy();
    }
}
