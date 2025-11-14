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

    [Header("Reload Indicator")]
    [SerializeField] private SpriteRenderer reloadIndicator;
    
    RangedController rangedController;
    MeleeController meleeController;
    PlayerController player;
    PlaceObject placeObject;
    AudioSource audioSource;

    // Cache for performance
    private bool lastArmState = false;
    private bool lastHandsState = false;
    private bool lastReloadIndicatorState = false;
    private bool wasChosen = false; // NEW: Track if player was chosen last frame
    private ItemSO lastSelectedItem = null;
    private float aiWeaponCheckInterval = 0.5f;
    private float aiWeaponCheckTimer = 0f;

    private void Awake()
    {
        if (TryGetComponent<GrabbablePlayer>(out var grabPosition))
            grabPosition?.OnGrabbed.AddListener(Grabbed);
        else
            Logger.Log("Couldn't Find GrabPosition!");
    }

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

    private void Update() 
    {
        // Detect player switch and reset reload indicator
        if (player.isChosen != wasChosen)
        {
            wasChosen = player.isChosen;
            
            if (!player.isChosen)
            {
                // Player was deselected - hide reload indicator
                if (reloadIndicator != null)
                {
                    reloadIndicator.enabled = false;
                }
            }
            
            // Always reset cache when switching players to force recalculation
            lastReloadIndicatorState = !player.isChosen; // Set opposite of current state to force update
        }
        
        // Only update reload indicator when player is chosen
        if (player.isChosen)
        {
            UpdateReloadIndicator();
        }
        
        // Optimize null item handling
        if (selectedItem == null) 
        {
            SetArmState(false);
            SetHandsState(false);
            
            if (player.isChosen) 
            {
                PlaceObject.active = false;
            }
        }
        
        if (meleeController.swinging) { return; }

        // AI players - check weapon switching less frequently
        if (!player.isChosen) 
        {
            aiWeaponCheckTimer += Time.deltaTime;
            if (aiWeaponCheckTimer >= aiWeaponCheckInterval)
            {
                aiWeaponCheckTimer = 0f;
                ChangeToWeapon();
            }
            return;
        }

        // Human player - handle input
        HandleNumberKeyInput();
    }

    private void HandleNumberKeyInput()
    {
        // Early exit if no keys pressed
        if (!Input.anyKeyDown) return;

        for (int i = 1; i <= 6; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + (i - 1)))
            {
                SlotChange(i);
                break;
            } 
        }
    }

    private void UpdateReloadIndicator()
    {
        if (reloadIndicator == null) return;

        bool shouldShow = false;
        
        if (selectedItem != null)
        {
            shouldShow =
                selectedItem.itemType == ItemType.Ranged &&
                selectedItem.ammo <= 0 &&           
                !selectedItem.reloading;
        }
        
        // Only update if state changed
        if (shouldShow != lastReloadIndicatorState)
        {
            lastReloadIndicatorState = shouldShow;
            reloadIndicator.enabled = shouldShow;
        }
    }
    
    private void Grabbed()
    {
        if (selectedItem == null) return;
        
        selectedItem.reloading = false;
        selectedItem.swung = false;
    }
    
    private void ChangeToWeapon() 
    {
        if (player.grabPlayer.IsGrabbed() || !player.playerHealth.alive) return;

        // Check if current weapon still has ammo/uses
        if (selectedItem != null) 
        {
            if (selectedItem.totalAmmo + selectedItem.ammo > 0 || selectedItem.uses > 0) 
            {
                return;
            }
        }

        // Try to find ranged weapon first
        for (int i = 0; i < inventory.Length; i++) 
        {
            ItemSO item = inventory[i];
            if (item == null) { continue; }

            if (item.itemType == ItemType.Ranged && item != selectedItem) 
            {
                SlotChange(i + 1);
                return;
            }
        }

        // If current slot has something, don't switch
        if (inventory[numSelect] != null) return;
        
        // Try to find melee weapon
        for (int i = 0; i < inventory.Length; i++) 
        {
            ItemSO item = inventory[i];
            if (item == null) { continue; }
            
            if (item.itemType == ItemType.Melee && item != selectedItem) 
            {
                SlotChange(i + 1);
                return;
            }
        }
    }

    private void SlotChange(int alpha)
    {
        SetArmState(false);
        SetHandsState(false);

        if (player.isChosen) 
        {
            PlaceObject.active = false;
        }

        numSelect = alpha - 1;
        selectedItem = inventory[numSelect];
        lastSelectedItem = selectedItem;

        if (inventory[numSelect] == null)
        {
            return;
        }

        switch (inventory[numSelect].itemType) 
        {
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

        if (inventory[numSelect].wearSFX != null) 
        {
            audioSource.PlayOneShot(inventory[numSelect].wearSFX);
        }
    }

    private void ChangeToRanged(int index) 
    {   
        SetArmState(true);

        muzzleFlash?.ZeroIntensity();

        rangedController.ChangeWeapon(inventory[index]);
    }

    private void ChangeToMelee(int index) 
    {
        SetHandsState(true);

        meleeController.ChangeWeapon(inventory[index]);
    }

    private void ChangeToConstruction(int index) 
    {
        PlaceObject.active = true;

        ItemSO slot = inventory[index];
        placeObject.ChangePlaceable(slot.placerSprite, slot.spawn, slot.radius);
    }

    public void ThrowWeapon() 
    {
        if (selectedItem == null) return;
        
        var thrownWeapon = Instantiate(throwaway, transform.position, head.rotation).GetComponent<PlayerThrow>();
        
        thrownWeapon.Init(inventory[numSelect]);

        inventory[numSelect] = null;
        selectedItem = null;
        lastSelectedItem = null;
    }

    public void GetWeapon(ItemSO weapon, GameObject obj) 
    {
        if (weapon.itemType == ItemType.Construction) 
        {
            for (int i = inventory.Length - 1; i > -1; i--) 
            {
                if (inventory[i] == null) 
                {
                    inventory[i] = weapon;
                    Destroy(obj);
                    return;
                }
            }            
        } 
        else 
        {
            for (int i = 0; i < inventory.Length; i++) 
            {
                if (inventory[i] == null) 
                {
                    inventory[i] = weapon;
                    Destroy(obj);
                    return;
                }
            }
        }
    }

    private void SetArmState(bool state)
    {
        if (lastArmState != state)
        {
            lastArmState = state;
            arm.SetActive(state);
        }
    }

    private void SetHandsState(bool state)
    {
        if (lastHandsState != state)
        {
            lastHandsState = state;
            hands.SetActive(state);
        }
    }
}