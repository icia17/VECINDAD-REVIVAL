using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChosenPlayerChanger : MonoBehaviour
{
    [HideInInspector]
    public PlayerController nextPlayer;

    public static bool changing = false;

    private void Start() {
        nextPlayer = GameObject.Find("Player2").GetComponent<PlayerController>();
    }

    public IEnumerator Change() {
        changing = true;

        PlaceObject.active = false;
        PlayerList[] players = PlayerListLists.players.ToArray();
        
        foreach(var player in players) {
            if (player.playerInventory.selectedItem == null) continue;
            
            player.playerInventory.selectedItem.reloading = false;
            player.playerInventory.selectedItem.swung = false;
        }

        if (players.Length == 1) {
            players[0].playerController.isChosen = true;
            changing = false;
            yield break;
        }

        int chosenIndex = -1;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].playerController.isChosen)
            {
                players[i].playerController.rb.constraints = RigidbodyConstraints2D.FreezeAll;
                players[i].playerController.changingPlayers = true;
                chosenIndex = i;
                break;
            }
        }

        yield return new WaitForSeconds(Time.deltaTime * 3);

        if (chosenIndex != -1)
        {
            int nextIndex = (chosenIndex + 1) % players.Length;

            if (!players[nextIndex].playerController.grabPlayer.IsGrabbed()) {
                players[chosenIndex].playerController.isChosen = false;
                players[nextIndex].playerController.isChosen = true;
                nextPlayer = players[nextIndex].playerController;
            }

            players[chosenIndex].playerController.changingPlayers = false;
            players[chosenIndex].playerController.rb.constraints = RigidbodyConstraints2D.None;
            players[chosenIndex].playerController.rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        }

        changing = false;
    }

}
