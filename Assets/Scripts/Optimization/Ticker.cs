using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour
{
    public static float tickTime = 0.2f;

    private float _tickerTimer;

    public static Action OnTickAction;

    private void Update() {
        _tickerTimer += Time.deltaTime;

        if (_tickerTimer >= tickTime) {
            _tickerTimer = 0;
            TickEvent();
        }
    }

    private void TickEvent() {
        OnTickAction?.Invoke();
    }
}
