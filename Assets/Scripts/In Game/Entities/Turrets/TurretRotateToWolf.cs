using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRotateToWolf : MonoBehaviour
{
    TurretLOSController los;

    private void Start() {
        los = GetComponent<TurretLOSController>();    
    }

    private void FixedUpdate() {
        if (los.closestWolf == null) { return; }
        
        Vector2 direction = los.closestWolf.transform.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        Quaternion targetRotation = Quaternion.Euler(0,0,angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 50);
    }
}