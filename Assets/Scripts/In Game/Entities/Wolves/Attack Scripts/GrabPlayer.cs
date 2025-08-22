using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class GrabPlayer : MonoBehaviour
{
    WolfLifeController wolfLifeController;
    RedWalkToTarget redWalkToTarget;

    [Header("Player layer")]
    public LayerMask layer;

    [Header("Attack hitbox stats")]
    public Transform attackPos;
    public float attackRadius;

    [Header("Attack damage and cooldown")]
    public float damage;
    public float damageCD;

    [HideInInspector]
    public bool grabbed = false;

    Collider2D grabbedPlayer;
    float baseDamageCD;

    private void Start() {
        wolfLifeController = GetComponentInParent<WolfLifeController>();    
        redWalkToTarget = GetComponentInParent<RedWalkToTarget>();

        baseDamageCD = damageCD;

        damageCD = 0.0f;
    }

    private void Update() {
        if (Attack() && !grabbed) {
            grabbed = true;
            Grab();
        }

        if (grabbed) {
            MoveGrabbedPlayer();
            DoDamage();
            CheckDestination();
        }

        if (grabbed && wolfLifeController.isDead && grabbedPlayer != null) {
            grabbed = false;

            grabbedPlayer.GetComponentInParent<GrabPosition>().FreePlayer();
        }
    }
    
    private bool Attack() {
        var isColliding = Physics2D.OverlapCircle(attackPos.position, attackRadius, layer);
        return isColliding;
    }

    private void Grab() {
        grabbedPlayer = Physics2D.OverlapCircle(attackPos.position, attackRadius, layer);

        PlayerInventory inv = grabbedPlayer.GetComponent<PlayerInventory>();

        if (inv == null) return;

        inv.selectedItem.reloading = false;
        inv.selectedItem.swung = false;
    }

    private void MoveGrabbedPlayer() {
        if (grabbedPlayer == null) {
            grabbed = false;
            return; 
        }

        GrabPosition grabPosition = grabbedPlayer.GetComponentInParent<GrabPosition>();

        if (!grabPosition.grabbed) {
            grabPosition.SetPosition(transform, !wolfLifeController.isDead);
        }
    }
    
    private void DoDamage() {
        damageCD -= Time.deltaTime;

        if (damageCD > 0 || grabbedPlayer == null) { return; }

        PlayerHealth playerHealth = grabbedPlayer.GetComponentInParent<PlayerHealth>();

        playerHealth.TakeDamage(damage);

        damageCD = baseDamageCD;
    }

    private void CheckDestination() {
        if (Vector2.Distance(transform.position, redWalkToTarget.closestExit.transform.position) < 0.1f && redWalkToTarget.goingToExit && grabbedPlayer != null) {
            PlayerHealth playerHealth = grabbedPlayer.GetComponentInParent<PlayerHealth>();

            if (playerHealth.health > 0) {
                playerHealth.TakeDamage(playerHealth.health);
            }
            
            wolfLifeController.wolfLife = 0.0f;
        }
    }

    // TODO: Remove after use

    #if UNITY_EDITOR

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }

    #endif
}