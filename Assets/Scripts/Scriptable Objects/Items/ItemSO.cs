using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    [Header("Global Properties")]
    public ItemType itemType;
    public Sprite itemSprite;
    public string itemName;
    public int itemPrice;
    public AudioClip wearSFX;

    [Header("Ranged Weapon Attributes")]
    public GameObject bullet;
    public Vector3 shootPos;
    public Vector3 screenShake;
    public float outerMuzzleLight;
    public float rate;
    public int ammo;
    public int unchangedAmmo;
    public int totalAmmo;
    public int unchangedTotalAmmo;
    public float reloadTime;
    public int itemId;
    public bool canFire;
    public bool reloading;
    public bool singleShot;
    public AudioClip reloadSFX;
    public AudioClip shootSFX;

    [Header("Ranged Weapon Inital Upgrade Cost")]
    public float fireRateCost;
    public float magSizeCost;
    public float maxCapacityCost;
    public float reloadTimeCost;

    [Header("Melee Weapon Attributes")]
    public Sprite heldSprite;
    public float swingSpeed;
    public float swingCooldown;
    public float damage;
    public float reach;
    public int uses;
    public bool swung;
    public AudioClip swingSFX;

    [Header("Placeable Object Attributes")]
    public GameObject spawn;
    public Sprite placerSprite;
    public float radius;
    public string buildSFX;

    public ItemSO Clone()
    {
        return (ItemSO)MemberwiseClone();
    }
}

public enum ItemType { Ranged, Melee, Construction }
