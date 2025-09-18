using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour
{
    [SerializeField] private float tickTime = 0.2f;

    private float _tickerTimer;

    public static Action OnTickAction;

    private void Update() {
        _tickerTimer += Time.deltaTime;

        if (_tickerTimer >= tickTime) {
            _tickerTimer -= tickTime;
            TickEvent();
        }
    }

    private void TickEvent() {
        OnTickAction?.Invoke();
    }
}
