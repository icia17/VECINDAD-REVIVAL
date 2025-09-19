using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.U2D;

public class WolfHealthController : MonoBehaviour
{
    [Header("Wolf Health Points")]
    public float wolfHealth;

    [Header("Wolf full body animator")]
    [SerializeField] private Animator animator;

    [SerializeField] private List<SpriteRenderer> sprites;
    
    [HideInInspector]
    public UnityEvent OnDeath;

    [HideInInspector]
    public PoolableObjectSO poolableType;
    
    private ParticleSystem particleSystem;
    private bool isDead = false;

    private float maxWolfHealth;
    
    private readonly string deathAnim = "Death";
    private readonly string hurtAnim = "Damaged";
    
    private void Awake() 
    {
        if (!TryGetComponent(out particleSystem))
            Logger.Log("ParticleSystem Not Found!");

        maxWolfHealth = wolfHealth;
    }
    
    public void InitializeWolf(Vector3 spawnPosition)
    {
        isDead = false;
        wolfHealth = maxWolfHealth;
        
        transform.position = spawnPosition;
        
        var agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.Warp(spawnPosition); 
            agent.ResetPath();   
            agent.nextPosition = spawnPosition;
        }

        foreach (var sprite in sprites)
        {
            sprite.color = Color.white;
        }

        animator.enabled = true;
        
        var collider = GetComponentInChildren<BoxCollider2D>();
        collider.enabled = true;
    }

    public void TakeDamage(float damage, bool splat) 
    {
        if (isDead) return; 

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
        if (isDead) return; 
        
        Die();
    }
    
    private void Die()
    {
        if (isDead) return; 
        
        isDead = true;
        
        WaveManager.Instance.OnWolfDeath();
        
        OnDeath?.Invoke();
        animator.Play(deathAnim);
    }
    
    public void PostMortem()
    {
        ObjectPooler.Instance.ReturnToPool(poolableType, this.gameObject);
    }
}