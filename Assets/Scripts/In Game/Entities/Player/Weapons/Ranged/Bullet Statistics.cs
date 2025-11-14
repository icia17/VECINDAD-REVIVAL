using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletStatistics : MonoBehaviour
{
    [Header("Rigidbody")]
    public Rigidbody2D rb;

    [Header("Bullet Statistics")]
    public float bulletSpeed;
    public float damage;

    [Header("Bullet Explosive Attributes")]
    public bool canExplode;

    [Header("SFX")]
    [SerializeField] AudioClip[] audioClips;

    public RangedController rangedController;
    public float bulletLifetime;
    
    [HideInInspector]
    public bool canSplatter = false;
    
    public PoolableObjectSO poolableBulletType { get; private set; }
    
    private CircleCollider2D circleCol;
    private ParticleSystem particleSys;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private PlayerHealth shooter;
    private AudioSource audioSource;
    private ParticleCollision particleCollision;

    private bool hasExploded = false;
    private bool isActive = false; // Track if bullet is currently in use

    private TurretRangedController turretRangedController;
    
    private float originalBulletLifetime;
    
    // Cached ParticleSystem modules to avoid repeated GetModule calls
    private ParticleSystem.CollisionModule cachedCollision;
    private ParticleSystem.MainModule cachedMain;
    private ParticleSystem.ShapeModule cachedShape;
    private bool particleModulesCached = false;

    private void Awake()
    {
        circleCol = GetComponent<CircleCollider2D>();
        particleSys = GetComponent<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        particleCollision = GetComponent<ParticleCollision>();
        
        if (canExplode)
        {
            animator = GetComponent<Animator>();
        }
        
        originalBulletLifetime = bulletLifetime;
        
        // Cache particle system modules once
        if (particleSys != null)
        {
            cachedCollision = particleSys.collision;
            cachedMain = particleSys.main;
            cachedShape = particleSys.shape;
            particleModulesCached = true;
        }
    }

    public void Init(RangedController rangedController) 
    {
        this.rangedController = rangedController;
        shooter = rangedController.playerHealth;
        poolableBulletType = rangedController.currentWeapon.bullet;

        isActive = true;
        ResetBullet();
    }
    
    public void InitTurret(TurretRangedController turretRangedController) 
    {
        this.turretRangedController = turretRangedController;
        poolableBulletType = turretRangedController.bullet;
        
        isActive = true;
        ResetBullet();
    }
    
    public void ResetBullet()
    {
        hasExploded = false;
        canSplatter = false;
        
        bulletLifetime = originalBulletLifetime;
        
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.constraints = RigidbodyConstraints2D.None;
        }
        
        if (circleCol != null)
        {
            circleCol.enabled = true;
        }
        
        if (particleSys != null)
        {
            particleSys.Stop();
            particleSys.Clear();
            
            if (particleModulesCached)
            {
                cachedCollision.enabled = true;
                cachedMain.startColor = Color.white;
            }
        }
        
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        
        if (animator != null && canExplode)
        {
            animator.Play("Default", 0, 0f); // Added layer and normalized time for performance
        }
        
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        
        if (particleCollision != null)
        {
            particleCollision.EndSplat(); 
        }
        
        rb.velocity = transform.up * bulletSpeed;
    }

    private void Update() 
    {
        // Only run lifetime check if bullet is active
        if (isActive)
        {
            Lifetime();
        }
    }

    private void Lifetime() 
    {
        if (hasExploded) { return; }

        bulletLifetime -= Time.deltaTime;

        if (bulletLifetime <= 0) 
        {
            isActive = false; // Mark as inactive
            
            if (rangedController != null) 
            {
                rangedController.ReleaseBulletFromPool(this);
            } 
            else if (turretRangedController != null) 
            {
                turretRangedController.ReleaseBulletFromPool(this);
            } 
            else 
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (circleCol == null || !isActive) { return; }
        
        circleCol.enabled = false;
        
        if (particleModulesCached)
        {
            cachedCollision.enabled = false;
        }
        
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        
        // Check if audio clip exists before playing
        if (audioClips != null && audioClips.Length > 0 && audioClips[0] != null)
        {
            audioSource.PlayOneShot(audioClips[0]);
        }

        ExplosionCheck();
        
        if (hasExploded) { return; }

        particleSys.Play();
        spriteRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (circleCol == null || !isActive) { return; }
        
        circleCol.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;

        canSplatter = true;

        if (particleModulesCached)
        {
            cachedMain.startColor = Color.red;
        }
        
        DoDamage(other);
        
        // Check if audio clip exists before playing
        if (audioClips != null && audioClips.Length > 1 && audioClips[1] != null)
        {
            audioSource.PlayOneShot(audioClips[1]);
        }

        ExplosionCheck();

        if (hasExploded) { return; }

        if (particleModulesCached)
        {
            cachedMain.startSize = Random.Range(0.15f, 0.25f);
            cachedMain.startSpeed = Random.Range(40f, 60f);
            cachedShape.rotation = Vector3.forward * 44f;
        }

        particleSys.Play();
        
        if (particleCollision != null)
        {
            particleCollision.BeginSplat();
        }

        spriteRenderer.enabled = false;
    }

    private void DoDamage(Collider2D other)
    {
        if (rangedController != null && shooter != null) 
        {
            shooter.health += damage * 0.1f; // Use multiplication instead of division
        }

        var wolfLife = other.GetComponentInParent<WolfHealthController>();
        
        if (wolfLife != null)
        {
            wolfLife.TakeDamage(damage, false);
        }
    }

    private void ExplosionCheck() 
    {
        if (canExplode && !hasExploded) 
        {
            // Check if audio clip exists before playing
            if (audioClips != null && audioClips.Length > 2 && audioClips[2] != null)
            {
                audioSource.PlayOneShot(audioClips[2]);
            }
            
            particleSys.Play();
            
            if (animator != null)
            {
                animator.Play("Explode", 0, 0f);
            }
            
            hasExploded = true;
        }
    }

    public void Destroy() 
    {
        isActive = false;
        
        if (rangedController != null)
        {
            rangedController.ReleaseBulletFromPool(this);
        }
    }
    
    private void OnDisable()
    {
        // Mark as inactive when pooled/disabled
        isActive = false;
    }
}