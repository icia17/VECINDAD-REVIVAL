using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;

public class TurretRangedController : MonoBehaviour
{
    public Light2D ammoLight;
    public GameObject bullet;
    public GameObject shootPos;
    public Sprite fullSprite;     
    public Sprite emptySprite;        
    public float ammo;
    public float shotCD;
    [SerializeField] AudioClip buildSFX;
    [SerializeField] AudioClip shootSFX;
    bool canShoot = true;
    Animator animator;
    TurretLOSController los;
    ObjectPool<BulletStatistics> bulletPool;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    [HideInInspector]
    public bool empty = false;

    private void Start() {
        los = GetComponent<TurretLOSController>();    

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();

        CreateBulletPool();

        audioSource.PlayOneShot(buildSFX);
    }

    private void CreateBulletPool() {
        bulletPool = new ObjectPool<BulletStatistics>(() => {
            GameObject bulletObj = Instantiate(bullet, shootPos.transform.position, shootPos.transform.rotation);
            BulletStatistics bulletStats = bulletObj.GetComponent<BulletStatistics>();
            bulletStats.InitTurret(this);
            return bulletStats;
        }, bullet => {
            bullet.gameObject.SetActive(true);
            bullet.transform.position = shootPos.transform.position;
            bullet.transform.rotation = shootPos.transform.rotation;
            bullet.rb.velocity = shootPos.transform.up * bullet.bulletSpeed; // Reset the velocity
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

    private void Update() {
        if (los.closestWolf == null) { return; }

        if (canShoot && ammo > 0) {
            StartCoroutine(Shoot());
        }

        if (ammo <= 0) {
            empty = true;
            spriteRenderer.sprite = emptySprite;
            ammoLight.color = Color.red;
        }
    }

    private IEnumerator Shoot() {
        audioSource.PlayOneShot(shootSFX);
        
        canShoot = false;
        
        ammo--;

        animator.Play("Shoot");

        BulletStatistics bullet = bulletPool.Get();
        bullet.InitTurret(this);

        yield return new WaitForSeconds(shotCD);
        canShoot = true;
    }
}
