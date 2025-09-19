using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class PlayerGrabber : MonoBehaviour, IGrabber
{
    RedWalkToTarget redWalkToTarget;

    [Header("Player layer")]
    public LayerMask layer;

    [Header("Attack hitbox stats")]
    public Transform attackPos;
    public float attackRadius;

    [Header("Attack damage and cooldown")]
    public float damage;
    public float damageCD;

    private IGrabbable grabbedPlayer;
    private float baseDamageCD;

    private void Awake()
    {
        if (TryGetComponent<WolfHealthController>(out var wolfHealthController))
            wolfHealthController.OnDeath.AddListener(() => { Destroy(this); });
        else
            Logger.Log("Wolf Health Controller Not Found!");
        
        if (!TryGetComponent(out redWalkToTarget))
            Logger.Log("Red Walk To Target Not Found!");
        
        baseDamageCD = damageCD;

        damageCD = 0.0f;
    }

    private void OnDestroy()
    {
        if (grabbedPlayer != null) 
            ForceRelease(false);
    }

    private void Update()
    {
        if (!IsGrabbing())
        {
            grabbedPlayer = TryGrabPlayer();
        }
        else
        {
            grabbedPlayer.Hold(transform.position);
            GripDamage();
            CheckDestination();
        }
    }
    
    private IGrabbable TryGrabPlayer()
    {
        Collider2D col = Physics2D.OverlapCircle(attackPos.position, attackRadius, layer);
        
        if (col != null)
        {
            var grabbable = col.GetComponentInParent<IGrabbable>();

            if (grabbable != null && !grabbable.IsGrabbed() && grabbable.CanGrab())
            {
                grabbable.Grab(this);
                grabbable.EnableGrab(false);
                return grabbable;
            }
        }

        return null;
    }
    
    public void ForceRelease(bool preventGrab)
    {
        grabbedPlayer?.Release();

        grabbedPlayer?.EnableGrab(!preventGrab);
        
        grabbedPlayer = null;
    }

    public bool IsGrabbing()
    {
        return grabbedPlayer != null;
    }
    
    private void GripDamage() {
        damageCD -= Time.deltaTime;

        if (damageCD <= 0)
        {
            grabbedPlayer?.Grip(damage);
            
            damageCD = baseDamageCD;
        }
    }

    private void CheckDestination() {
        if (Vector2.Distance(transform.position, redWalkToTarget.closestExit.transform.position) < 0.1f &&
            redWalkToTarget.goingToExit && grabbedPlayer != null)
        {
            grabbedPlayer.Squash();
            
            if (TryGetComponent<WolfHealthController>(out var wolfHealthController))
                wolfHealthController.TakeMaxDamage();
            else
                Logger.Log("Wolf Health Controller Not Found!");
        }
    }
}