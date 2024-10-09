using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    float speed = 150f;                     // Speed of the coin's movement
    float reachedDistance = 0.5f;           // Distance to consider the coin has reached the target
    int value;

    private Transform currencyIcon;         // Reference to the currency icon UI element
    private Vector3 targetPosition;         // Target position for the coin (currency icon)

    void Start()
    {
        // Find the currency icon in the scene
        currencyIcon = GameObject.Find("CurrencyIcon").transform;

        // Get the target position (currency icon in world space)
        targetPosition = currencyIcon.position;
    }

    void Update()
    {
        // Update the target position in case the currency icon moves (optional)
        targetPosition = currencyIcon.position;  
    }

    void DashToCurrencyIcon()
    {
        // Move towards the target using the dash movement logic
        Vector3 direction = (targetPosition - transform.position).normalized;  // Normalize to keep speed constant
        transform.position += direction * speed * Time.deltaTime;

        // Check if the coin has reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < reachedDistance)
        {
            // Coin has reached the currency icon
            transform.position = targetPosition;  // Snap to the exact position
            OnCoinCollected();
        }
    }

    void OnCoinCollected()
    {
        // Add currency to the player's total using the BattleCurrencyManager instance
        if (BattleCurrencyManager.instance != null)
        {
            BattleCurrencyManager.instance.AddCurrency(value);  // Add the coin's value to the currency
        }

        // Return coin to the pool
        CoinPooler.instance.ReturnCoinToPool(gameObject);
    }

    // Method to activate the coin when it's spawned
    public void ActivateCoin(Vector3 startPosition, int amount, float stayDuration)
    {
        transform.position = startPosition;     // Set the start position
        value = amount;                         // Set the value for the coin
        gameObject.SetActive(true);             // Activate the coin

        // Start the coroutine to move the coin
        StartCoroutine(MoveCoinToCurrencyIcon(stayDuration));
    }

    private IEnumerator MoveCoinToCurrencyIcon(float stayDuration)
    {
        // Wait for given number of seconds
        yield return new WaitForSeconds(stayDuration);

        // Now move to the currency icon
        while (Vector3.Distance(transform.position, targetPosition) > reachedDistance)
        {
            DashToCurrencyIcon();
            yield return null;  // Wait for the next frame
        }
    }
}
