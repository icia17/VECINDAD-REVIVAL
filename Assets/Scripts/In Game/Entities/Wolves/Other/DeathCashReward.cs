using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathCashReward : MonoBehaviour
{
    [Header("Base Cash Received Upon Death")]
    public int cashReward;

    private void Awake()
    {
        if (TryGetComponent(out WolfHealthController lifeController))
            lifeController.OnDeath.AddListener(OnDeath); 
        else
            Logger.Log("Couldn't subscribe to WolfLifeController's OnDeathEvent, Component Not Found!");
    }

    private void OnDeath()
    {
        GameManager.Add(cashReward);
    }
}
