using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleCurrencyManager : MonoBehaviour
{
    public static BattleCurrencyManager instance;
    public CurrencyIconAnimator currencyIconAnimator;       // Reference to the CurrencyIconAnimator script
    public TextMeshPro currencyText;                        // Reference to the UI text element
    private int currentCurrency = 0;

    private void Awake()
    {
        // Implement Singleton pattern to ensure there's only one instance of CurrencyManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // If there's another instance, destroy this one
        }
    }

    private void Start()
    {
        UpdateCurrencyUI();
    }

    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        Debug.Log("Currency added! Total Currency: " + currentCurrency);
        currencyIconAnimator.OnCurrencyAdded();
        UpdateCurrencyUI();
    }

    public int GetCurrency()
    {
        return currentCurrency;
    }

    private void UpdateCurrencyUI()
    {
        currencyText.text = currentCurrency.ToString();
    }
}
