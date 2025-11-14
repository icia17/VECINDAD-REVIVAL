using System.Collections;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    [Header("Current Weapon Properties")]
    public ItemSO currentWeapon;
    [SerializeField] private SpriteRenderer swingSpriteRenderer;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [HideInInspector] public bool swinging = false;
    
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerInventory inv;
    private PlayerController player;
    private PlayerLOSController los;

    private const float AI_SWING_DISTANCE = 2.5f;
    private const float SWING_DURATION = 0.5f;

    private void Awake() 
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        inv = GetComponentInParent<PlayerInventory>();
        player = GetComponentInParent<PlayerController>();
        los = GetComponentInParent<PlayerLOSController>();
    }

    private void FixedUpdate() 
    {
        if (currentWeapon == null) return;
        
        CheckWeaponDurability();

        if (player != null && !player.isChosen)
        {
            AISwing();
        }
    }

    private void CheckWeaponDurability()
    {
        if (currentWeapon.uses <= 0)
        {
            swinging = false;
            
            if (inv != null && inv.numSelect < inv.inventory.Length)
            {
                inv.inventory[inv.numSelect] = null;
                inv.selectedItem = null;
            }
        }
    }

    private void AISwing() 
    {
        if (currentWeapon.swung || los == null || los.closestWolf == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, los.closestWolf.transform.position);
        
        if (distanceToTarget < AI_SWING_DISTANCE)
        {
            Swing();
        }
    }

    public void ChangeWeapon(ItemSO weaponChosen) 
    {
        if (weaponChosen == null) return;

        currentWeapon = weaponChosen;
        currentWeapon.swung = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = currentWeapon.heldSprite;
        }

        if (swingSpriteRenderer != null)
        {
            swingSpriteRenderer.sprite = null;
        }
    }

    public void Swing() 
    {
        if (currentWeapon == null || currentWeapon.swung) return;

        StartCoroutine(OnSwing());
    }

    private IEnumerator OnSwing() 
    {
        if (audioSource != null && currentWeapon.swingSFX != null)
        {
            audioSource.PlayOneShot(currentWeapon.swingSFX);
        }
        
        if (player != null)
        {
            player.swinging = true;
        }

        currentWeapon.swung = true;
        swinging = true;

        if (swingSpriteRenderer != null)
        {
            swingSpriteRenderer.sprite = currentWeapon.itemSprite;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = null;
        }

        if (animator != null)
        {
            animator.Play("Swing");
        }

        yield return new WaitForSeconds(SWING_DURATION);

        swinging = false;
        
        if (player != null)
        {
            player.swinging = false;
        }

        yield return new WaitForSeconds(currentWeapon.swingCooldown);

        if (currentWeapon != null)
        {
            currentWeapon.swung = false;
        }
    }

    public void SwingOff() 
    {
        if (currentWeapon == null || spriteRenderer == null) return;

        spriteRenderer.sprite = currentWeapon.heldSprite;
        
        if (swingSpriteRenderer != null)
        {
            swingSpriteRenderer.sprite = null;
        }
    }
}