using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;
using Cinemachine;

public class RangedController : MonoBehaviour
{
    [Header("Shooting Position Properties")]
    [SerializeField] Light2D muzzleFlash;
    public Transform shootPos;

    [Header("Current Weapon Properties")]
    public ItemSO currentWeapon;
    public SpriteRenderer spriteRenderer;

    [Header("Player Health")]
    public PlayerHealth playerHealth;

    [Header("Audio Source")]
    [SerializeField] AudioSource audioSource;
    
    [Header("Player Rotate To Wolf Script")]
    [SerializeField] PlayerRotateToWolf rotWolf;

    [Header("Cinemachine Impulse Source")]
    [SerializeField] CinemachineImpulseSource cis;
    
    PlayerController player;
    PlayerLOSController los;
    
    // Cache for optimization
    private Coroutine shootCoroutine;
    private Coroutine reloadCoroutine;
    private WaitForSeconds[] cachedFireRateWaits;
    private WaitForSeconds cachedReloadWait;
    private const int MAX_CACHED_FIRE_RATES = 10;
    private Dictionary<float, WaitForSeconds> fireRateCache = new Dictionary<float, WaitForSeconds>();

    private void Awake() 
    {
        player = GetComponent<PlayerController>();
        los = GetComponent<PlayerLOSController>();
    }

    private void FixedUpdate() 
    {
        if (currentWeapon == null) { return; }
        if (currentWeapon.itemType != ItemType.Ranged) { return; }
        
        if (rotWolf.armThisFrame) 
        {
            currentWeapon.canFire = true;
            rotWolf.armThisFrame = false;   
        }

        // AI shooting logic - only for non-chosen players
        if (!player.isChosen && los.closestWolf != null) 
        {
            if (currentWeapon.ammo > 0) 
            {
                Shoot();
            } 
            else if (currentWeapon.totalAmmo > 0) // Only reload if there's ammo to reload with
            {
                Reload();
            }
        }
    }

    public void ChangeWeapon(ItemSO weaponChosen) 
    {
        if (weaponChosen == null) return; 

        currentWeapon = weaponChosen;

        Logger.Log("WP CHANGE");

        spriteRenderer.sprite = currentWeapon.itemSprite;
        currentWeapon.reloading = false;
        shootPos.localPosition = currentWeapon.shootPos;

        if (cis != null) 
        {
            cis.m_DefaultVelocity = currentWeapon.screenShake;
        }
        
        muzzleFlash.pointLightOuterRadius = currentWeapon.outerMuzzleLight;
        
        if (currentWeapon.ammo > 0) 
        {
            currentWeapon.canFire = true;
        }
        
        // Pre-cache the reload wait for this weapon
        cachedReloadWait = GetOrCreateWaitForSeconds(currentWeapon.reloadTime);
    }

    public void Shoot() 
    {
        if (!currentWeapon.canFire || currentWeapon.reloading) { return; }

        // Stop existing shoot coroutine if running (prevents stacking)
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
        }
        
        shootCoroutine = StartCoroutine(OnShoot());
    }

    public void Reload() 
    {
        if (currentWeapon.reloading || currentWeapon.totalAmmo == 0) { return; }

        // Stop existing reload coroutine if running (prevents stacking)
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
        }
        
        reloadCoroutine = StartCoroutine(OnReload());
    }
    
    private void SpawnBullet() 
    {
        GameObject bulletObj = ObjectPooler.Instance.SpawnFromPool(
            currentWeapon.bullet, 
            shootPos.position, 
            shootPos.rotation
        );
        
        if (bulletObj != null)
        {
            BulletStatistics bulletStats = bulletObj.GetComponent<BulletStatistics>();
            if (bulletStats != null)
            {
                bulletStats.Init(this);
            }
        }
    }

    public void ReleaseBulletFromPool(BulletStatistics bullet) 
    {
        if (bullet != null && bullet.poolableBulletType != null)
        {
            ObjectPooler.Instance.ReturnToPool(bullet.poolableBulletType, bullet.gameObject);
        }
    }

    private IEnumerator OnShoot() 
    {
        // Play shoot sound
        if (currentWeapon.shootSFX != null && audioSource != null) 
        {
            audioSource.PlayOneShot(currentWeapon.shootSFX);
        }

        currentWeapon.canFire = false;
        currentWeapon.ammo--;

        SpawnBullet();
        ShootVFX();

        // Use cached WaitForSeconds to avoid GC allocation
        WaitForSeconds fireRateWait = GetOrCreateWaitForSeconds(currentWeapon.rate);
        yield return fireRateWait;

        // Check if we're out of ammo
        if (currentWeapon.ammo <= 0 || (currentWeapon.totalAmmo <= 0 && currentWeapon.ammo <= 0)) 
        { 
            shootCoroutine = null;
            yield break; 
        }

        currentWeapon.canFire = true;
        shootCoroutine = null;
    }
    
    private void ShootVFX() 
    {
        if (player.isChosen && cis != null) 
        {
            cis.GenerateImpulse();
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.intensity = 9;
        }
    }
    
    private IEnumerator OnReload() 
    {
        // Play reload sound
        if (currentWeapon.reloadSFX != null && audioSource != null) 
        {
            audioSource.PlayOneShot(currentWeapon.reloadSFX);
        }

        currentWeapon.canFire = false;
        currentWeapon.reloading = true;

        // Use cached WaitForSeconds
        yield return cachedReloadWait;

        int ammoNeeded = currentWeapon.unchangedAmmo - currentWeapon.ammo;

        if (currentWeapon.totalAmmo >= ammoNeeded) 
        {
            currentWeapon.totalAmmo -= ammoNeeded;
            currentWeapon.ammo += ammoNeeded;
        } 
        else 
        {
            currentWeapon.ammo += currentWeapon.totalAmmo;
            currentWeapon.totalAmmo = 0;
        }

        currentWeapon.reloading = false;
        currentWeapon.canFire = true;
        reloadCoroutine = null;
    }
    
    // Cache WaitForSeconds objects to avoid GC allocations
    private WaitForSeconds GetOrCreateWaitForSeconds(float time)
    {
        // Round to 2 decimal places to avoid cache misses from floating point precision
        float roundedTime = Mathf.Round(time * 100f) / 100f;
        
        if (!fireRateCache.ContainsKey(roundedTime))
        {
            // Limit cache size to prevent memory bloat
            if (fireRateCache.Count >= MAX_CACHED_FIRE_RATES)
            {
                // Clear oldest entries (simple approach - could use LRU if needed)
                fireRateCache.Clear();
            }
            
            fireRateCache[roundedTime] = new WaitForSeconds(roundedTime);
        }
        
        return fireRateCache[roundedTime];
    }
    
    private void OnDisable()
    {
        // Clean up coroutines when disabled
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
        
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
    }
}