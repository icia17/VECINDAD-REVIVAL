using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;

public class TurretRangedController : MonoBehaviour
{
    public Light2D ammoLight;
    public PoolableObjectSO bullet;
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
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    [HideInInspector]
    public bool empty = false;

    private void Start() {
        los = GetComponent<TurretLOSController>();    

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(buildSFX);
    }

    private void SpawnBullet() {
        GameObject bulletObj = ObjectPooler.Instance.SpawnFromPool(bullet, shootPos.transform.position, shootPos.transform.rotation);
        
        BulletStatistics bulletStats = bulletObj.GetComponent<BulletStatistics>();
        bulletStats.InitTurret(this);
    }

    public void ReleaseBulletFromPool(BulletStatistics bullet) {
        ObjectPooler.Instance.ReturnToPool(bullet.poolableBulletType, bullet.gameObject);
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

        SpawnBullet();

        yield return new WaitForSeconds(shotCD);
        canShoot = true;
    }
}