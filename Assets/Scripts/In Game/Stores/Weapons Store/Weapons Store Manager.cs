using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsStoreManager : MonoBehaviour
{
    [Header("Weapons List")]
    [SerializeField] List<ItemSO> weapons;

    [Header("Item Holder and Name")]
    [SerializeField] Image holder;
    [SerializeField] TextMeshProUGUI tmp;
    
    [Header("Buttons")]
    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject backButton;

    [Header("Locked Screen and Prize Tag")]
    [SerializeField] GameObject locked;
    [SerializeField] TextMeshProUGUI priceTag;

    [Header("Upgrade Texts")]
    [SerializeField] TextMeshProUGUI rateUpgradeText;
    [SerializeField] TextMeshProUGUI magUpgradeText;
    [SerializeField] TextMeshProUGUI maxCapUpgradeText;
    [SerializeField] TextMeshProUGUI reloadUpgradeText;

    [Header("Upgrades Prize Tags")]
    [SerializeField] TextMeshProUGUI rateUpgradeCost;
    [SerializeField] TextMeshProUGUI magUpgradeCost;
    [SerializeField] TextMeshProUGUI maxCapUpgradeCost;
    [SerializeField] TextMeshProUGUI reloadUpgradeCost;
    

    [Header("Single Shot Non-Upgradeables")]
    [SerializeField] GameObject rateUpgrade;
    [SerializeField] GameObject magUpgrade;

    ItemSO currentWeapon;
    int currentIndex = 0;
    bool weaponFound = false;

    public void Open() {
        SearchWeapon();
        HideSingleShotUpgrades();
        ShowNewWeapon();
    }

    private void SearchWeapon() {
        weaponFound = false;
        foreach(var item in PlayerListLists.chosenPlayer.playerInventory.inventory) {
            if (item == null) { continue; }

            if (item.itemName == weapons[currentIndex].itemName) {
                currentWeapon = item;
                weaponFound = true;
            }
        }

        if (!weaponFound) {
            currentWeapon = weapons[currentIndex];
            locked.SetActive(true);
            priceTag.text = "$" + weapons[currentIndex].itemPrice.ToString();
        } else {
            locked.SetActive(false);
        }

    }

    private void HideSingleShotUpgrades() {
        if (currentWeapon.singleShot) {
            rateUpgrade.SetActive(false);
            magUpgrade.SetActive(false);
        } else {
            rateUpgrade.SetActive(true);
            magUpgrade.SetActive(true);
        }
    }

    private void ShowNewWeapon() {
        holder.sprite = currentWeapon.itemSprite;
        tmp.text = currentWeapon.itemName;

        
        rateUpgradeText.text = "* Cadencia de disparo: " + Mathf.Round(currentWeapon.rate * 100) / 100 + "s";
        magUpgradeText.text = "* Capacidad de cartucho: " + currentWeapon.unchangedAmmo + " balas";
        maxCapUpgradeText.text = "* Munición máxima: " + currentWeapon.unchangedTotalAmmo + " balas";
        reloadUpgradeText.text = "* Tiempo de recarga: " + Mathf.Round(currentWeapon.reloadTime * 100) / 100 + "s";

        if (!currentWeapon.singleShot) {
            rateUpgradeCost.text = "$" + Mathf.RoundToInt(currentWeapon.fireRateCost);
            magUpgradeCost.text = "$" + Mathf.RoundToInt(currentWeapon.magSizeCost);
        }

        maxCapUpgradeCost.text = "$" + Mathf.RoundToInt(currentWeapon.maxCapacityCost);
        reloadUpgradeCost.text = "$" + Mathf.RoundToInt(currentWeapon.reloadTimeCost);
    }

    public void PreviousWeapon() {
        AudioManager.Instance.PlaySFX("Click");

        currentIndex--;

        CheckButtons();

        Open();
    }

    public void NextWeapon() {
        AudioManager.Instance.PlaySFX("Click");
        
        currentIndex++;

        CheckButtons();

        Open();
    }

    public void UnlockWeapon() {
        if (GameManager.cash < weapons[currentIndex].itemPrice) return;

        ItemSO[] inv = PlayerListLists.chosenPlayer.playerInventory.inventory;

        for (int i = 0; i < inv.Length; i++) {
            if (inv[i] == null) {
                inv[i] = weapons[currentIndex].Clone();
                GameManager.cash -= weapons[currentIndex].itemPrice;
                AudioManager.Instance.PlaySFX("Kaching");
                break;
            }
        }

        Open();
    }

    public void UpgradeRate() {
        if (GameManager.cash < Mathf.RoundToInt(currentWeapon.fireRateCost)) return;

        currentWeapon.rate -= currentWeapon.rate * 0.1f;

        GameManager.cash -= Mathf.RoundToInt(currentWeapon.fireRateCost);

        currentWeapon.fireRateCost *= 1.25f;

        AudioManager.Instance.PlaySFX("Kaching");

        Open();
    }

    public void UpgradeMagazine() {
        if (GameManager.cash < Mathf.RoundToInt(currentWeapon.magSizeCost)) return;

        currentWeapon.unchangedAmmo++;
        currentWeapon.ammo = currentWeapon.unchangedAmmo;

        GameManager.cash -= Mathf.RoundToInt(currentWeapon.magSizeCost);

        currentWeapon.magSizeCost *= 1.25f;

        AudioManager.Instance.PlaySFX("Kaching");

        Open();
    }

    public void UpgradeMaxCapacity() {
        if (GameManager.cash < Mathf.RoundToInt(currentWeapon.maxCapacityCost)) return;

        currentWeapon.unchangedTotalAmmo++;
        currentWeapon.totalAmmo = currentWeapon.unchangedTotalAmmo;

        GameManager.cash -= Mathf.RoundToInt(currentWeapon.maxCapacityCost);

        currentWeapon.maxCapacityCost *= 1.25f;

        AudioManager.Instance.PlaySFX("Kaching");

        Open();
    }

    public void UpgradeReload() {
        if (GameManager.cash < Mathf.RoundToInt(currentWeapon.reloadTimeCost)) return;

        currentWeapon.reloadTime -= currentWeapon.reloadTime * 0.1f;

        GameManager.cash -= Mathf.RoundToInt(currentWeapon.reloadTimeCost);

        currentWeapon.reloadTimeCost *= 1.25f;

        AudioManager.Instance.PlaySFX("Kaching");

        Open();
    }

    private void CheckButtons() {
        if (currentIndex + 1 >= weapons.Count) {
            nextButton.SetActive(false);
        } else {
            nextButton.SetActive(true);
        }

        if (currentIndex - 1 < 0) {
            backButton.SetActive(false);
        } else {
            backButton.SetActive(true);
        } 
    }
}
