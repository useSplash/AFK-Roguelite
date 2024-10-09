using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPooler : MonoBehaviour
{
    public static CoinPooler instance;         // Singleton instance for global access
    public GameObject coinPrefab;              // Coin prefab to instantiate
    public int poolSize = 30;                  // Initial pool size

    private Queue<GameObject> coinPool = new Queue<GameObject>();

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Populate the pool with coins
        for (int i = 0; i < poolSize; i++)
        {
            GameObject coin = Instantiate(coinPrefab);
            coin.SetActive(false);  // Deactivate the coin
            coinPool.Enqueue(coin);  // Add to the pool
        }
    }

    // Method to get a coin from the pool
    public GameObject GetPooledCoin()
    {
        // Check if there are any available coins in the pool
        if (coinPool.Count > 0)
        {
            // Dequeue a coin from the pool and return it
            GameObject coin = coinPool.Dequeue();
            return coin;
        }

        // Optionally instantiate a new coin if the pool is empty (expandable pool)
        GameObject newCoin = Instantiate(coinPrefab);
        newCoin.SetActive(false);  // Deactivate the new coin
        return newCoin;
    }

    // Method to return the coin to the pool
    public void ReturnCoinToPool(GameObject coin)
    {
        coin.SetActive(false);  // Deactivate the coin
        coinPool.Enqueue(coin);  // Enqueue the coin back to the pool
    }
}
