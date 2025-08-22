using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BonusListManager : MonoBehaviour
{
    List<GeneralBonus> bonuses;
    public static float bonus = 1;

    private void Start() {
        bonuses = GetComponentsInChildren<GeneralBonus>().ToList();    
    }

    private void OnEnable() {
        Ticker.OnTickAction += Tick;
    }

    private void OnDisable() {
        Ticker.OnTickAction -= Tick;
    }

    private void Tick() {
        bonus = 1;

        bonuses.Sort();

        for (int i = 0; i < bonuses.Count; i++) {
            bonuses[i].gameObject.transform.SetSiblingIndex(i);

            if (bonuses[i].bonus <= 1) continue;

            bonus += bonuses[i].bonus;
        }
        
        if (bonus > 1) bonus--;
    }
}
