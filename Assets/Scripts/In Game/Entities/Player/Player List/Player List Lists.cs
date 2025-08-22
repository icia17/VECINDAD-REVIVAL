using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerListLists : MonoBehaviour
{
    [SerializeField] List<PlayerList> playerLists;
    public static List<PlayerList> players;
    public static PlayerList chosenPlayer;

    private void Start() {
        players = playerLists;
        
        for(int i = 0; i < players.Count; i++) {
            if (players[i].playerController.isChosen) {
                chosenPlayer = players[i];
                break;
            }
        }
    }

    private void Update() {
        for(int i = 0; i < players.Count; i++) {
            if (players[i].playerController.isChosen) {
                chosenPlayer = players[i];
                break;
            }
        }
    }
}
