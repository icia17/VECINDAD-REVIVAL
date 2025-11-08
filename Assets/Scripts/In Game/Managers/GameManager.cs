using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("---------------- UI OBJECTS ----------------")]
    [SerializeField] GameObject loseMenu;
    [SerializeField] GameObject hud;

    bool activateDeath = false;

    [HideInInspector] 
    public static GameState State;

    [HideInInspector]
    public static float cash;

    [HideInInspector]
    public static bool inStore = false;

    private void Start() {
        cash = 3000;
        AudioManager.Instance.audioMixer.SetFloat("lowpass", 5000);
        ChosenPlayerChanger.changing = false;    
    }

    private void FixedUpdate() {
        if (PlayerListLists.players.Count != 0 || activateDeath) return;

        loseMenu.SetActive(true);
        hud.SetActive(false);

        LoseMenuController loseMenuController = loseMenu.GetComponent<LoseMenuController>();

        loseMenuController.OpenAnimation();

        activateDeath = true;
    }

    public void UpdateGameState(GameState state) {
        State = state;
    }

    public void StoreState(bool state) {
        inStore = state;
    }
    public static void Add(float prizeMoney) {
        cash += prizeMoney * BonusListManager.bonus; 
    }
}


public enum GameState {
    Wave,
    Timer,
    Lose,
    Tutorial
}
