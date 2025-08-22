using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WolfPrizeMoney : MonoBehaviour
{
    [Header("Wolf base prize money")]
    public int prizeMoney;

    public void OnDeath() {
        GameManager.Add(prizeMoney);
    }
}
