using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GeneralBonus : MonoBehaviour, IComparable<GeneralBonus>
{
    [SerializeField] BonusType Type;
    [SerializeField] Transform bloodHolder;
    public float bonus = 1;
    TextMeshProUGUI tmp;
    
    private void Start() {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        Ticker.OnTickAction += Tick;
    }

    private void OnDisable() {
        Ticker.OnTickAction -= Tick;
    }

    private void Tick() {
        switch (Type) {
            case BonusType.Overcharge:
                OverchargeBonus();
                break;
            case BonusType.Blood:
                BloodBonus();
                break;
            case BonusType.Alive:
                AliveBonus();
                break;
            case BonusType.Wave:
                WaveBonus();
                break;
        }
    }

    void OverchargeBonus()
    {
        float multiplier = 1;
        foreach (var player in PlayerListLists.players) {
            if (player.playerHealth.bloodLust) {
                multiplier += 0.5f;
            }
        }

        bonus = multiplier;

        if (bonus > 1) {
            tmp.text = "Bonus Sobrecarga: " + bonus + "x";
        } else {
            tmp.text = "";
        }
    }

    void BloodBonus() {
        bonus = bloodHolder.childCount / 100f;

        bonus = Mathf.Round(bonus * 100f) / 100f;

        if (bonus > 2) bonus = 2; 
        
        if (bonus > 1) {
            tmp.text = "Bonus Sangre: " + bonus + "x";
        } else {
            bonus = 1;

            tmp.text = "";
        }
    }

    void AliveBonus()
    {
        bonus = PlayerListLists.players.Count;

        if (bonus > 1) {
            tmp.text = "Bonus Vivos: " + bonus + "x";
        } else {
            tmp.text = "";
        }
    }

    void WaveBonus() {
        float multiplier = WaveManager.wave/10;
        bonus = 1 + multiplier;

        if (bonus > 1) {
            tmp.text = "Bonus Oleada: " + bonus + "x";
        } else {
            tmp.text = "";
        }
    }

    public int CompareTo(GeneralBonus other)
    {
        if (other == null) return 1;

        return other.bonus.CompareTo(bonus);
    }
}

public enum BonusType {
    Overcharge,
    Blood,
    Alive,
    Wave,
}
