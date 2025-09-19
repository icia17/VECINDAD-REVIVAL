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
    private bool isDead = false;

    private readonly string deathAnim = "Death";
    private readonly string hurtAnim = "Damaged";
    
    private void Awake() 
    {
        if (!TryGetComponent(out particleSystem))
            Debug.Log("ParticleSystem Not Found!");
    }
    
    public void InitializeWolf()
    {
        isDead = false;
    }

    public void TakeDamage(float damage, bool splat) 
    {
        if (isDead) return; // Prevent processing if already dead

        wolfHealth -= damage;

        if (splat)
            particleSystem.Play();
        
        if (wolfHealth <= 0) {
            Die();
        }
        else
            animator.Play(hurtAnim); 
    }

    public void TakeMaxDamage()
    {
        if (isDead) return; // Prevent processing if already dead
        
        Die();
    }
    
    private void Die()
    {
        if (isDead) return; // Prevent multiple death calls
        
        isDead = true;
        
        WaveManager.Instance.OnWolfDeath();
        
        OnDeath?.Invoke();
        animator.Play(deathAnim);
    }
    
    public void PostMortem() 
    {
        Destroy(gameObject);
    }
}