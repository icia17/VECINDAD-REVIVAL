using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableReactions : MonoBehaviour
{
    PlayerController playerController;
    public UsesLeft currentAmmoBox;
    public bool nearAmmoBox = false;

    private void Start() {
        playerController = GetComponent<PlayerController>();    
    }

    private void Update() {
        if (currentAmmoBox == null) {
            nearAmmoBox = false;
        }    
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!playerController.isChosen) return;
        
        if (other.gameObject.tag == "AmmoBox") {
            currentAmmoBox = other.GetComponent<UsesLeft>(); 

            nearAmmoBox = true;
            other.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        if (!playerController.isChosen) return;

        if (other.gameObject.tag == "AmmoBox") {
            nearAmmoBox = false;
            other.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
