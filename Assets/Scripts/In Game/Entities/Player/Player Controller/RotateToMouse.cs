using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [Header("Rotation Time and Degrees Removal System")]
    public float rotationTime;
    public bool removeDegrees;

    [Header("Arm Only Properties")]
    [SerializeField] public bool isArm = false;

    PlayerController player;
    Vector2 mousePos;

    private void Awake() {
        player = GetComponentInParent<PlayerController>();    
    }

    private void FixedUpdate() {
        if (player.isChosen) {
            Rotate();
        } else if (isArm) {
            transform.localRotation = Quaternion.Euler(0,0,93);
        }
    }
    
    private void Rotate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg + Remove90Degrees();

        Quaternion targetRotation = Quaternion.Euler(0,0,angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationTime * Time.deltaTime);
    }

    private int Remove90Degrees() {
        if (removeDegrees) {
            return -90;
        }
        else {
            return 0;
        }
    }
}
