using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPosition : MonoBehaviour
{
    Transform wolfTransform;

    [Header("Objects to deactivate")]
    public GameObject arm;
    public Collider2D col;

    [HideInInspector]
    public bool grabbed = false;
    
    public void SetPosition(Transform wolf, bool alive) {
        wolfTransform = wolf;

        grabbed = alive;

        if (grabbed) {
            arm.SetActive(false);
            col.enabled = false;
        }
    }

    public void FreePlayer() {
        grabbed = false;
        arm.SetActive(true);
        col.enabled = true;
    }

    private void Update() {
        if (grabbed) {
            transform.position = wolfTransform.position;
        }
    }

}
