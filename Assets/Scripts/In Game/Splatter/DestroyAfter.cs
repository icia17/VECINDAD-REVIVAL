using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float resizeAmount;

    private float value = 1;
    private float baseValue;

    private PoolableObjectSO poolableObject;
    
    private void OnEnable() {
        Ticker.OnTickAction += Tick;
    }
    
    private void OnDisable() {
        Ticker.OnTickAction -= Tick;
    }

    private void Awake()
    {
        baseValue = value;
    }

    public void Init(PoolableObjectSO poolableObject)
    {
        this.poolableObject = poolableObject;
        value = baseValue;
    }
    
    void Tick() {
        value -= resizeAmount;

        transform.localScale = Vector3.one * value;
        
        if (value < 0) {
            ObjectPooler.Instance.ReturnToPool(poolableObject, gameObject);
        }
    }
}
