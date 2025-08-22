using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("General")]
    public ItemSO[] inventory;
    public ItemSO selectedItem;
    public int numSelect;

    [Header("Ranged Arm")]
    [SerializeField] public GameObject arm;

    [Header("Melee hands")]
    [SerializeField] public GameObject hands;

    [Header("Throwaway Prefab and Head Transform")]
    [SerializeField] Transform head;
    [SerializeField] GameObject throwaway;
    
    [Header("Muzzle Flash")]
    [SerializeField] MuzzleFlash muzzleFlash;

    RangedController rangedController;
    MeleeController meleeController;
    PlayerController player;
    PlaceObject placeObject;
    AudioSource audioSource;

    private void Start()
    {   
        rangedController = GetComponent<RangedController>();
        player = GetComponent<PlayerController>();
        meleeController = GetComponentInChildren<MeleeController>();
        placeObject = FindObjectOfType<PlaceObject>();
        audioSource = GetComponent<AudioSource>();

        // Ensure inventory items are clones of the base items
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null)
            {
                inventory[i] = inventory[i].Clone();
            }
        }
    }

    private void Update() {
        if (selectedItem == null) {
            arm.SetActive(false);
            hands.SetActive(false);
            
            if (player.isChosen) {
                PlaceObject.active = false;
            }
        }
        
        if (meleeController.swinging) { return; }

        if (!player.isChosen) {
            ChangeToWeapon();
            return;
        }

        for (int i = 1; i <= 6; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + (i - 1)))
            {
                SlotChange(i);
                break;
            } 
        }
    }

    private void ChangeToWeapon() {
        if (player.grabPlayer.grabbed || !player.playerHealth.alive) return;

        if (selectedItem != null) {
            if (selectedItem.totalAmmo + selectedItem.ammo > 0 || selectedItem.uses > 0) {
                return;
            }
        }

        for (int i = 0; i < inventory.Length; i++) {
            if (inventory[i] == null) { continue; }

            if (inventory[i].itemType == ItemType.Ranged && inventory[i] != selectedItem) {
                SlotChange(i + 1);
                return;
            }
        }

        if (inventory[numSelect] != null) return;
        
        for (int i = 0; i < inventory.Length; i++) {
            if (inventory[i] == null) { continue; }
            
            if (inventory[i].itemType == ItemType.Melee && inventory[i] != selectedItem) {
                SlotChange(i + 1);
                return;
            }
        }
    }

    private void SlotChange(int alpha)
    {
        arm.SetActive(false);
        hands.SetActive(false);

        if (player.isChosen) {
            PlaceObject.active = false;
        }

        numSelect = alpha - 1;
        selectedItem = inventory[numSelect];

        if (inventory[numSelect] == null)
        {
            return;
        }

        switch (inventory[numSelect].itemType) {
            case ItemType.Ranged:
                ChangeToRanged(numSelect);
                break;
            case ItemType.Melee:
                ChangeToMelee(numSelect);
                break;
            case ItemType.Construction:
                ChangeToConstruction(numSelect);
                break;
        }

        if (inventory[numSelect].wearSFX != null) {
            audioSource.PlayOneShot(inventory[numSelect].wearSFX);
        }
    }

    private void ChangeToRanged(int index) {   
        arm.SetActive(true);

        muzzleFlash.ZeroIntensity();

        rangedController.ChangeWeapon(inventory[index]);
    }

    private void ChangeToMelee(int index) {
        hands.SetActive(true);

        meleeController.ChangeWeapon(inventory[index]);
    }

    private void ChangeToConstruction(int index) {
        PlaceObject.active = true;

        ItemSO slot = inventory[index];
        placeObject.ChangePlaceable(slot.placerSprite, slot.spawn, slot.radius);
    }

    public void ThrowWeapon() {
        if (selectedItem == null) return;
        
        var thrownWeapon = Instantiate(throwaway, transform.position, head.rotation).GetComponent<PlayerThrow>();
        
        thrownWeapon.Init(inventory[numSelect]);

        inventory[numSelect] = null;
        selectedItem = null;
    }

    public void GetWeapon(ItemSO weapon, GameObject obj) {
        if (weapon.itemType == ItemType.Construction) {
            for (int i = inventory.Length - 1; i > -1; i--) {
                if (inventory[i] == null) {
                    inventory[i] = weapon;
                    Destroy(obj);
                    break;
                }
            }            
        } else {
            for (int i = 0; i < inventory.Length; i++) {
                if (inventory[i] == null) {
                    inventory[i] = weapon;
                    Destroy(obj);
                    break;
                }
            }
        }
    }
}