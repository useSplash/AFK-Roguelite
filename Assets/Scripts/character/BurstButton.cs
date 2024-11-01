using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BurstButton : MonoBehaviour
{
    Character character;
    public Slider energySlider;
    public Image burstBarImage;
    public Image characterPortrait;
    private bool isOscillating;
    private Coroutine oscillationCoroutine;
    private Color originalColor;

    public void InitializeBurstButton(Character _character)
    {
        if (_character == null)
        {
            characterPortrait.sprite = null;
            UpdateSlider(0);
            return;
        }
        character = _character;
        characterPortrait.sprite = character.characterData.characterSprite;
        character.burstButton = this;
        originalColor = burstBarImage.color;
        character.GainEnergy(0);
    }

    // Update the slider and start/stop oscillation based on energy
    public void UpdateSlider(float energyPercentage)
    {
        energySlider.value = energyPercentage;

        if (energyPercentage >= 1.0f && !isOscillating)
        {
            // Start the opacity oscillation when the bar is full
            isOscillating = true;
            oscillationCoroutine = StartCoroutine(OscillateBarOpacity());
        }
        else if (energyPercentage < 1.0f && isOscillating)
        {
            // Stop the oscillation when the bar is no longer full
            isOscillating = false;
            StopCoroutine(oscillationCoroutine);
            ResetBurstBarColor();  // Reset color to full opacity
        }
    }

    // Coroutine to oscillate the burst bar opacity between full and half opacity
    private IEnumerator OscillateBarOpacity()
    {
        float minOpacity = 0.5f;
        float maxOpacity = 1.0f;
        float duration = 0.8f; // Time to complete one oscillation

        Color barColor = burstBarImage.color;

        while (true)
        {
            // Fade from maxOpacity to minOpacity
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(maxOpacity, minOpacity, t / duration);
                burstBarImage.color = new Color(barColor.r, barColor.g, barColor.b, alpha);
                yield return null;
            }

            // Fade from minOpacity back to maxOpacity
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(minOpacity, maxOpacity, t / duration);
                burstBarImage.color = new Color(barColor.r, barColor.g, barColor.b, alpha);
                yield return null;
            }
        }
    }

    // Reset the burst bar to full opacity
    private void ResetBurstBarColor()
    {
        burstBarImage.color = originalColor;
    }

    public void ActivateBurst()
    {
        // Band aid Solution
        if (energySlider.value == 0) return;
        if (!character.IsDead() && character != null)
        {
            character?.ActivateBurst();
        }
    }
}
