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

    private TurretRangedController turretRangedController;
    
    private float originalBulletLifetime;

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
    }

    public void Init(RangedController rangedController) {
        this.rangedController = rangedController;
        shooter = rangedController.playerHealth;
        poolableBulletType = rangedController.currentWeapon.bullet;

        ResetBullet();
    }
    
    public void InitTurret(TurretRangedController turretRangedController) {
        this.turretRangedController = turretRangedController;
        poolableBulletType = turretRangedController.bullet;
        
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
            
            var collision = particleSys.collision;
            collision.enabled = true;
            
            var main = particleSys.main;
            main.startColor = Color.white;
        }
        
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        
        if (animator != null && canExplode)
        {
            animator.Play("Default"); 
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

    private void Update() {
        Lifetime();
    }

    private void Lifetime() {
        if (hasExploded) { return; }

        bulletLifetime -= Time.deltaTime;

        if (bulletLifetime <= 0) {

            if (rangedController != null) {
                rangedController.ReleaseBulletFromPool(this);
            } else if (turretRangedController != null) {
                turretRangedController.ReleaseBulletFromPool(this);
            } else {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (circleCol == null) { return; }
        
        circleCol.enabled = false;
        
        var collision = particleSys.collision;
        collision.enabled = false;
        
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        
        audioSource.PlayOneShot(audioClips[0]);

        ExplosionCheck();
        
        if (hasExploded) { return; }

        particleSys.Play();

        spriteRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (circleCol == null) { return; }
        
        circleCol.enabled = false;

        rb.constraints = RigidbodyConstraints2D.FreezePosition;

        var main = particleSys.main;
        var shape = particleSys.shape;

        canSplatter = true;

        main.startColor = Color.red;
        
        DoDamage(other);
        
        audioSource.PlayOneShot(audioClips[1]);

        ExplosionCheck();

        if (hasExploded) { return; }

        main.startSize = Random.Range(0.15f,0.25f);
        main.startSpeed = Random.Range(40f,60f);
        shape.rotation = Vector3.forward * 44f;

        particleSys.Play();
        particleCollision.BeginSplat();

        spriteRenderer.enabled = false;
    }

    private void DoDamage(Collider2D other)
    {
        if (rangedController != null) {
            shooter.health += damage/10;
        }

        var wolfLife = other.GetComponentInParent<WolfHealthController>();

        wolfLife.TakeDamage(damage, false);
    }

    private void ExplosionCheck() {
        if (canExplode && !hasExploded) {
            audioSource.PlayOneShot(audioClips[2]);
            particleSys.Play();
            animator.Play("Explode");
            hasExploded = true;
        }
    }

    public void Destroy() {
        rangedController.ReleaseBulletFromPool(this);
    }
}