using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

public class MeleeController : MonoBehaviour
{
    [Header("Current Weapon Properties")]
    public ItemSO currentWeapon;
    [SerializeField] SpriteRenderer swingSpriteRenderer;

    [Header("Audio Source")]
    [SerializeField] AudioSource audioSource;

    [HideInInspector] public bool swinging = false;
    
    Animator animator;
    SpriteRenderer spriteRenderer;
    PlayerInventory inv;
    PlayerController player;
    PlayerLOSController los;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();    
        inv = GetComponentInParent<PlayerInventory>();
        animator = GetComponent<Animator>();
        player = GetComponentInParent<PlayerController>();
        los = GetComponentInParent<PlayerLOSController>();
    }

    private void FixedUpdate() {
        if (currentWeapon == null) { return; }    
        
        if (currentWeapon.uses <= 0) {
            swinging = false;
            inv.inventory[inv.numSelect] = null;
            inv.selectedItem = null;
        }

        if (!player.isChosen) {
            AISwing();
        }
    }

    private void AISwing() {
        if (currentWeapon.swung || los.closestWolf == null) return;

        if (Vector3.Distance(transform.position, los.closestWolf.transform.position) < 2.5f) {
            Swing();
        }
    }

    public void ChangeWeapon(ItemSO weaponChosen) {
        if (weaponChosen == null) return; 

        currentWeapon = weaponChosen;

        spriteRenderer.sprite = currentWeapon.heldSprite;
        swingSpriteRenderer.sprite = null;

        currentWeapon.swung = false;
    }

    public void Swing() {
        if (currentWeapon.swung) return;

        StartCoroutine(OnSwing());
    }

    private IEnumerator OnSwing() {
        audioSource.PlayOneShot(currentWeapon.swingSFX);
        
        player.swinging = true;
        currentWeapon.swung = true;

        swinging = true;

        swingSpriteRenderer.sprite = currentWeapon.itemSprite;
        spriteRenderer.sprite = null;

        animator.Play("Swing");

        yield return new WaitForSeconds(0.5f);

        swinging = false;
        player.swinging = false;

        yield return new WaitForSeconds(currentWeapon.swingCooldown);

        currentWeapon.swung = false;
    }

    public void SwingOff() {
        spriteRenderer.sprite = currentWeapon.heldSprite;
        swingSpriteRenderer.sprite = null;
    }
}
