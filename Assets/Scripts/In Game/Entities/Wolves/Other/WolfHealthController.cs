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
    [SerializeField] private LineRenderer lineRenderer;
    
    [HideInInspector]
    public UnityEvent OnDeath;

    [HideInInspector]
    public PoolableObjectSO poolableType;
    
    private ParticleSystem particleSystem;
    private bool isDead = false;

    private float maxWolfHealth;

    private readonly string idleAnim = "Idle";
    private readonly string deathAnim = "Death";
    private readonly string hurtAnim = "Damaged";

    private Animator[] otherAnimators;
    private BoxCollider2D collider;
    private NavMeshAgent agent;
    
    private void Awake() 
    {
        if (!TryGetComponent(out particleSystem))
            Logger.Log("ParticleSystem Not Found!");

        maxWolfHealth = wolfHealth;
        
        otherAnimators= GetComponentsInChildren<Animator>();
        collider = GetComponentInChildren<BoxCollider2D>();
        agent = GetComponent<NavMeshAgent>();
    }
    
    public void InitializeWolf(Vector3 spawnPosition)
    {
        isDead = false;
        wolfHealth = maxWolfHealth;
        
        transform.position = spawnPosition;
        
        if (agent != null)
        {
            agent.Warp(spawnPosition); 
            agent.ResetPath();   
            agent.nextPosition = spawnPosition;
        }

        StartCoroutine(InitDelay());
    }

    private IEnumerator InitDelay()
    {
        yield return new WaitForSeconds(0.5f);
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
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

    private void ResetAnim()
    {
        animator.Play(hurtAnim);
        
        foreach (var sprite in sprites)
        {
            sprite.color = Color.white;
        }
        
        collider.enabled = true;
        
        foreach (var anim in otherAnimators)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        lineRenderer.startColor = Color.clear;
        lineRenderer.endColor = Color.clear;
    }
    
    public void PostMortem()
    {
        ResetAnim();
        ObjectPooler.Instance.ReturnToPool(poolableType, this.gameObject);
    }
}