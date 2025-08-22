using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MeleeStoreManager : MonoBehaviour
{
    [Header("Melees List")]
    [SerializeField] List<ItemSO> melees;

    [Header("Item Holder and Name")]
    [SerializeField] Image holder;
    [SerializeField] TextMeshProUGUI tmp;

    [Header("Buttons")]
    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject buy;

    [Header("Price Tag and Uses")]
    [SerializeField] TextMeshProUGUI priceTag;
    [SerializeField] TextMeshProUGUI durability;

    [Header("Item Lock")]
    [SerializeField] GameObject locked;

    ItemSO currentMelee;
    int currentIndex = 0;

    public static int stock = 2;

    public void ShowNewMelee() {
        currentMelee = melees[currentIndex];

        holder.sprite = currentMelee.itemSprite;
        tmp.text = currentMelee.itemName;
        durability.text = "Durabilidad: " + currentMelee.uses;
        priceTag.text = "$" + currentMelee.itemPrice.ToString();

        if (stock <= 0) {
            locked.SetActive(true);
        } else {
            locked.SetActive(false);
        }
    }

    public void Buy() {
        if (GameManager.cash < Mathf.RoundToInt(currentMelee.itemPrice)) return;

        ItemSO[] inv = PlayerListLists.chosenPlayer.playerInventory.inventory;

        for (int i = 0; i < inv.Length; i++) {
            if (inv[i] == null) {
                inv[i] = currentMelee.Clone();
                
                stock--;
                
                GameManager.cash -= Mathf.RoundToInt(currentMelee.itemPrice);

                AudioManager.Instance.PlaySFX("Kaching");
                break;
            }
        }

        ShowNewMelee();
    }

    public void PreviousMelee() {
        AudioManager.Instance.PlaySFX("Click");

        currentIndex--;

        CheckButtons();

        ShowNewMelee();
    }

    public void NextMelee() {
        AudioManager.Instance.PlaySFX("Click");
        
        currentIndex++;

        CheckButtons();

        ShowNewMelee();
    }

    private void CheckButtons() {
        if (currentIndex + 1 >= melees.Count) {
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
