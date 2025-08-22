using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySelection : MonoBehaviour
{
    [SerializeField] Image[] selection;
    [SerializeField] Image[] image;
    [SerializeField] Sprite selectedSprite;
    [SerializeField] Sprite unselectedSprite;
    ItemSO[] inventory;
    int numSelect;

    private void FixedUpdate() {
        if (PlayerListLists.chosenPlayer == null) {
            return;
        }

        inventory = PlayerListLists.chosenPlayer.playerInventory.inventory; 
        numSelect = PlayerListLists.chosenPlayer.playerInventory.numSelect;
        
        for(int i = 0; i < image.Length; i++) {
            if (i == numSelect) {
                selection[i].sprite = selectedSprite;
            } else {
                selection[i].sprite = unselectedSprite;
            }

            if (inventory[i] == null) {
                image[i].color = Color.clear;
                continue; 
            }
            
            image[i].color = Color.white;
            image[i].sprite = inventory[i].itemSprite;
        }
    }
}
