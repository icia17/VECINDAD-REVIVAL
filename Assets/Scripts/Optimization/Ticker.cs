using System;
using UnityEngine;

public class Ticker : MonoBehaviour
{
    [SerializeField] private float tickTime = 0.2f;

    private float _tickerTimer;

    public static Action OnTickAction;

    private void FixedUpdate() 
    {
        _tickerTimer += Time.fixedDeltaTime;

        while (_tickerTimer >= tickTime) 
        {
            _tickerTimer -= tickTime;
            TickEvent();
        }
    }

    private void TickEvent() 
    {
        OnTickAction?.Invoke();
    }
}