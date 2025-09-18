using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WolfHealthController : MonoBehaviour
{
    [Header("Wolf Health Points")]
    public float wolfHealth;

    [Header("Wolf full body animator")]
    [SerializeField] private Animator animator;
    
    [HideInInspector]
    public UnityEvent OnDeath;

    private ParticleSystem particleSystem;

    private readonly string deathAnim = "Death";
    private readonly string hurtAnim = "Damaged";
    
    private void Awake() 
    {
        if (!TryGetComponent(out particleSystem))
            Debug.Log("ParticleSystem Not Found!");
    }

    public void TakeDamage(float damage, bool splat) 
    {
        wolfHealth -= damage;

        if (splat)
            particleSystem.Play();
        
        if (wolfHealth <= 0) {
            OnDeath?.Invoke();
            
            WaveManager.wolvesLeft--;

            animator.Play(deathAnim);  
        }
        else
            animator.Play(hurtAnim); 
    }

    public void TakeMaxDamage()
    {
        WaveManager.wolvesLeft--;
            
        OnDeath?.Invoke();

        animator.Play(deathAnim);  
    }
    
    public void PostMortem() 
    {
        Destroy(gameObject);
    }
}
