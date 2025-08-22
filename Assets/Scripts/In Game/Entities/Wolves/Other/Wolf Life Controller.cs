using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WolfLifeController : MonoBehaviour
{
    [Header("Wolf life points")]
    public float wolfLife;

    [Header("Wolf full body animator")]
    public Animator animator;

    [Header("Death unity event")]
    public UnityEvent OnDeath;

    [HideInInspector] 
    public bool isDead = false;

    ParticleSystem particles;

    private void Start() {
        particles = GetComponent<ParticleSystem>();
    }

    public void TakeDamage(float damage) {
        wolfLife -= damage;

        animator.Play("Damaged"); 
    }

    public void TakeSplatDamage(float damage) {
        wolfLife -= damage;

        particles.Play();
        
        animator.Play("Damaged"); 
    }
    
    private void Update() {
        if (wolfLife <= 0 && !isDead) {
            isDead = true;

            WaveManager.wolvesLeft--;
            
            OnDeath?.Invoke();

            animator.Play("Death");  
        }
    }

    public void PostMortem() {
        Destroy(gameObject);
    }
}
