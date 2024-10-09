using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPooler : MonoBehaviour
{
    public static EnemyPooler instance;  // Singleton instance for global access
    public GameObject enemyPrefab;       // Enemy prefab to instantiate
    public int poolSize = 10;            // Initial pool size

    private Queue<GameObject> enemyPool = new Queue<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Populate the pool with inactive enemies
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);  // Initially inactive
            enemyPool.Enqueue(enemy);  // Add to the pool
        }
    }

    // Method to get an enemy from the pool
    public GameObject GetPooledEnemy()
    {
        if (enemyPool.Count > 0)
        {
            GameObject enemy = enemyPool.Dequeue();
            enemy.SetActive(true);  // Activate the enemy
            return enemy;
        }
        else
        {
            // If no enemies are available in the pool, instantiate a new one
            return null;
        }
    }

    // Method to return an enemy back to the pool
    public void ReturnEnemyToPool(GameObject enemy)
    {
        enemy.SetActive(false);  // Deactivate the enemy
        enemyPool.Enqueue(enemy);  // Add back to the pool
    }
}
