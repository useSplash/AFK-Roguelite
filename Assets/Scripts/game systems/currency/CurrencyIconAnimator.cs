using System.Collections;
using UnityEngine;

public class CurrencyIconAnimator : MonoBehaviour
{
    RectTransform currencyIconRectTransform;    // Reference to the RectTransform of the currency icon
    float scaleIncrease = 1.3f;                 // How much to increase the size
    float scaleDuration = 0.13f;                 // How long the scale effect lasts (up and down)

    private Vector3 originalScale;              // Store the original scale of the currency icon

    private void Awake()
    {
        currencyIconRectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        // Store the original scale of the currency icon
        originalScale = currencyIconRectTransform.localScale;
    }

    // Call this method when currency is added to trigger the scale effect
    public void OnCurrencyAdded()
    {
        // Stop any currently running scale animations
        StopAllCoroutines();
        StartCoroutine(AnimateScale());
    }

    private IEnumerator AnimateScale()
    {
        // Animate the currency icon to scale up
        float elapsedTime = 0f;
        Vector3 targetScale = originalScale * scaleIncrease;

        while (elapsedTime < scaleDuration)
        {
            currencyIconRectTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the icon reaches the exact target scale
        currencyIconRectTransform.localScale = targetScale;

        // Wait for a moment (optional: how long the icon stays large)
        yield return new WaitForSeconds(0.1f);

        // Animate the currency icon back to its original size
        elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            currencyIconRectTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the icon reaches the exact original scale
        currencyIconRectTransform.localScale = originalScale;
    }
}