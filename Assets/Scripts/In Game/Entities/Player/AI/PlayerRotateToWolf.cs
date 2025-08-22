using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerRotateToWolf : MonoBehaviour
{
    PlayerInventory playerInventory;
    PlayerLOSController los;
    PlayerController player;
    NavMeshAgent agent;
    GrabPosition grab;  

    [Header("Rotation Time")]
    [SerializeField] float rotateTime;

    [Header("Arm Show Object")]
    [SerializeField] GameObject arm;

    [Header("Hands Show Object")]
    [SerializeField] GameObject hands;

    [Header("Show Object Check")]
    public bool show = false;

    [HideInInspector] public bool armThisFrame = false;

    RangedController ranged;

    private void Start() {
        playerInventory = GetComponentInParent<PlayerInventory>();
        los = GetComponentInParent<PlayerLOSController>();    
        player = GetComponentInParent<PlayerController>();
        agent = GetComponentInParent<NavMeshAgent>();
        grab = GetComponentInParent<GrabPosition>();
        ranged = GetComponentInParent<RangedController>();
    }

    private void FixedUpdate() {
        if (player.isChosen) { return; }

        Vector2 direction = Target() - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        Quaternion targetRotation = Quaternion.Euler(0,0,angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateTime);

        if (!show) return;

        if (playerInventory.selectedItem != null) {
            switch (playerInventory.selectedItem.itemType) {
                case ItemType.Ranged:
                    ShowArm();
                    break;
                default:
                    break;
            }
        }
    }

    private void ShowArm()
    {
        if (los.closestWolf == null) {
            if (ranged != null && arm.activeSelf) {
                ranged.currentWeapon.reloading = false;
            }

            arm.SetActive(false);
            hands.SetActive(false);
        } else if (!arm.activeSelf && !grab.grabbed) {
            armThisFrame = true;
            arm.SetActive(true);
            hands.SetActive(false);
        }
    }

    private Vector3 Target() {
        if (los.closestWolf == null) {
            return agent.nextPosition;
        } else {
            return los.closestWolf.transform.position;
        }
    }
}