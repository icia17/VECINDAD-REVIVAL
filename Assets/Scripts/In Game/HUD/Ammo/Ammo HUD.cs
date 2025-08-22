using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoHUD : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    private void FixedUpdate() { 
        if (PlayerListLists.chosenPlayer == null) {
            tmp.text = "";
            return; 
        }
        
        ItemSO inv = PlayerListLists.chosenPlayer.playerInventory.selectedItem;

        if (inv == null) {
            tmp.text = "";
        } else if (inv.reloading) {
            tmp.text = "Recargando..."; 
        } else if (inv.itemType == ItemType.Ranged) {
            tmp.text = "Munición: " + inv.ammo + "/" + inv.totalAmmo;
        } else if (inv.itemType == ItemType.Construction) {
            tmp.text = "Construir: click izquierdo";
        } else if (inv.itemType == ItemType.Melee) {
            tmp.text = "Durabilidad: " + inv.uses;
        }
    }
}
