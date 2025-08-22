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

    ObjectPool<BulletStatistics> bulletPool;
    PlayerController player;
    PlayerLOSController los;

    private void Awake() {
        player = GetComponent<PlayerController>();
        los = GetComponent<PlayerLOSController>();

        CreateBulletPool();
    }
    
    private void FixedUpdate() {
        if (currentWeapon == null) { return; }
        if (currentWeapon.itemType != ItemType.Ranged) { return; }
        
        if (rotWolf.armThisFrame) {
            currentWeapon.canFire = true;
            rotWolf.armThisFrame = false;   
        }

        if (!player.isChosen && los.closestWolf != null) {
            if (currentWeapon.ammo > 0) {
                Shoot();
            } else {
                Reload();
            }
        }
    }

    public void ChangeWeapon(ItemSO weaponChosen) {
        if (weaponChosen == null) return; 

        currentWeapon = weaponChosen;

        spriteRenderer.sprite = currentWeapon.itemSprite;

        currentWeapon.reloading = false;

        shootPos.localPosition = currentWeapon.shootPos;

        if (cis != null) {
            cis.m_DefaultVelocity = currentWeapon.screenShake;
        }
        
        muzzleFlash.pointLightOuterRadius = currentWeapon.outerMuzzleLight;
        
        if (currentWeapon.ammo > 0) {
            currentWeapon.canFire = true;
        }
    }

    public void Shoot() {
        if (!currentWeapon.canFire || currentWeapon.reloading) { return; }

        StartCoroutine(OnShoot());
    }

    public void Reload() {
        if (currentWeapon.reloading || currentWeapon.totalAmmo == 0) { return; }

        StartCoroutine(OnReload());
    }
    
    private void CreateBulletPool() {
        bulletPool = new ObjectPool<BulletStatistics>(() => {
            GameObject bulletObj = Instantiate(currentWeapon.bullet, shootPos.position, shootPos.rotation);
            BulletStatistics bulletStats = bulletObj.GetComponent<BulletStatistics>();
            bulletStats.Init(this);
            return bulletStats;
        }, bullet => {
            bullet.gameObject.SetActive(true);
            bullet.transform.position = shootPos.position;
            bullet.transform.rotation = shootPos.rotation;
            bullet.rb.velocity = shootPos.up * bullet.bulletSpeed; // Reset the velocity
        }, bullet => {
            bullet.gameObject.SetActive(false);
        }, bullet => {
            Destroy(bullet.gameObject);
        });
    }

    public void ReleaseBulletFromPool(BulletStatistics bullet) {
        bulletPool.Release(bullet);
    }

    public void DestroyBulletFromPool(BulletStatistics bullet) {
        Destroy(bullet.gameObject);
    }

    public IEnumerator OnShoot() {
        if (currentWeapon.shootSFX != null) {
            audioSource.PlayOneShot(currentWeapon.shootSFX);
        }

        currentWeapon.canFire = false;

        currentWeapon.ammo--;

        BulletStatistics bullet = bulletPool.Get();
        bullet.Init(this);

        ShootVFX();

        yield return new WaitForSeconds(currentWeapon.rate);

        if (currentWeapon.ammo <= 0 || (currentWeapon.totalAmmo <= 0 && currentWeapon.ammo <= 0)) { yield break; }

        currentWeapon.canFire = true;
    }
    
    void ShootVFX() {
        if (player.isChosen) {
            cis.GenerateImpulse();
        }

        muzzleFlash.intensity = 9;
    }
    
    public IEnumerator OnReload() {
        if (currentWeapon.shootSFX != null) {
            audioSource.PlayOneShot(currentWeapon.reloadSFX);
        }

        currentWeapon.canFire = false;
        currentWeapon.reloading = true;

        yield return new WaitForSeconds(currentWeapon.reloadTime);

        int ammoNeeded = currentWeapon.unchangedAmmo - currentWeapon.ammo;

        if (currentWeapon.totalAmmo >= ammoNeeded) {

            currentWeapon.totalAmmo -= ammoNeeded;
            currentWeapon.ammo += ammoNeeded;

        } else {

            currentWeapon.ammo += currentWeapon.totalAmmo;
            currentWeapon.totalAmmo = 0;

        }

        currentWeapon.reloading = false;
        currentWeapon.canFire = true;
    }
}