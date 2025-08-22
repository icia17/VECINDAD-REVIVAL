using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildsStoreManager : MonoBehaviour
{
    [Header("Builds List")]
    [SerializeField] List<ItemSO> builds;

    [Header("Item Holder and Name")]
    [SerializeField] Image holder;
    [SerializeField] TextMeshProUGUI tmp;

    [Header("Buttons")]
    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject buy;

    [Header("Price Tag")]
    [SerializeField] TextMeshProUGUI priceTag;

    [Header("Item Lock")]
    [SerializeField] GameObject locked;

    ItemSO currentBuild;
    int currentIndex = 0;

    public static int stock = 2;

    public void ShowNewBuild() {
        currentBuild = builds[currentIndex];

        holder.sprite = currentBuild.itemSprite;
        tmp.text = currentBuild.itemName;
        priceTag.text = "$" + currentBuild.itemPrice.ToString();

        if (stock <= 0) {
            locked.SetActive(true);
        } else {
            locked.SetActive(false);
        }
    }

    public void Buy() {
        if (GameManager.cash < Mathf.RoundToInt(currentBuild.itemPrice)) return;

        ItemSO[] inv = PlayerListLists.chosenPlayer.playerInventory.inventory;

        for (int i = inv.Length - 1; i > -1; i--) {
            if (inv[i] == null) {
                inv[i] = currentBuild.Clone();
                
                stock--;
                
                GameManager.cash -= Mathf.RoundToInt(currentBuild.itemPrice);

                AudioManager.Instance.PlaySFX("Kaching");
                break;
            }
        }

        ShowNewBuild();
    }

    public void PreviousBuild() {
        AudioManager.Instance.PlaySFX("Click");

        currentIndex--;

        CheckButtons();

        ShowNewBuild();
    }

    public void NextBuild() {
        AudioManager.Instance.PlaySFX("Click");

        currentIndex++;

        CheckButtons();

        ShowNewBuild();
    }

    private void CheckButtons() {
        if (currentIndex + 1 >= builds.Count) {
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
