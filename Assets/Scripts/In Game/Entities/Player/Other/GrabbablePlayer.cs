using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GrabbablePlayer : MonoBehaviour, IGrabbable
{
    [Header("Objects to deactivate")]
    public GameObject arm;
    
    [HideInInspector]
    public UnityEvent OnGrabbed;
    public UnityEvent OnSquashed;

    private bool _grabbed;
    private bool _canGrab = true;
    
    private IGrabber _grabber;

    private PlayerHealth _playerHealth;
    
    private void Awake()
    {
        if (TryGetComponent(out _playerHealth))
            _playerHealth.OnDeath.AddListener(() => _grabber?.ForceRelease(false));
        else
            Debug.Log("Player Health Not Found!");
    }

    public void Grab(IGrabber grabber)
    {
        _grabbed = true;

        _grabber = grabber;
        
        arm.SetActive(false);
        
        OnGrabbed?.Invoke();
    }

    public void Hold(Vector3 grabberPosition)
    {
        transform.position = grabberPosition;
    }
    
    public void Release()
    {
        _grabbed = false;

        _grabber = null;
        
        arm?.SetActive(true);
    }

    public void Squash()
    {
        _grabbed = false;

        _grabber?.ForceRelease(true);
        
        OnSquashed?.Invoke();
        
        Destroy(this);
    }

    public void Grip(float damage)
    {
        _playerHealth?.TakeDamage(damage);
    }

    public void EnableGrab(bool enable)
    {
        _canGrab = enable;
    }

    public bool CanGrab()
    {
        return _canGrab;
    }
    
    public bool IsGrabbed()
    {
        return _grabbed;
    }

}
