using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestTarget : MonoBehaviour
{
    [Header("Is Wolf Capable of Invisibility?")]
    public bool invisible;

    [HideInInspector]
    public PlayerList closestPlayer;

    private void Start() {
        closestPlayer = FindObjectOfType<PlayerList>();
    }

    private void OnEnable() {
        Ticker.OnTickAction += Tick;
    }

    private void OnDisable() {
        Ticker.OnTickAction -= Tick;
    }

    private void Tick() {
        foreach(var player in PlayerListLists.players) {
            if (closestPlayer == null) {
                closestPlayer = player;
            }

            if (Vector2.Distance(transform.position, player.transform.position) < Vector2.Distance(transform.position, closestPlayer.transform.position)) {
                closestPlayer = player;
            }
        }
    }
}
