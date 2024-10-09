using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class BurstAnimationHandler : MonoBehaviour
{
    public static BurstAnimationHandler instance { get; private set; }
    public Animator animator;
    public TextMeshPro burstText;
    public TextMeshPro burstTextShadow;
    public Image burstImage;
    
    private Action onBurstAnimationEnd;   // Event to trigger when animation ends

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void PlayBurstAnimation(string attackName, Sprite attackerImage, Action onAnimationEnd)
    {
        onBurstAnimationEnd = onAnimationEnd; 
        burstImage.sprite = attackerImage;
        burstText.text = attackName;
        burstTextShadow.text = attackName;
        animator.Play("burst_animation", 0, 0);
    } 

    public void OnBurstComplete()
    {
        onBurstAnimationEnd?.Invoke();
        animator.Play("burst_idle");
    }
}
