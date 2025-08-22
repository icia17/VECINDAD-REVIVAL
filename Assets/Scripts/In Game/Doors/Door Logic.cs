using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLogic : MonoBehaviour
{
    [Header("Door hitbox size")]
    public Vector2 doorHitbox;

    [Header("Door Top and Bottom")]
    public Transform topDoor;
    public Transform botDoor;

    [Header("Player Layer")]
    public LayerMask playerLayer;
    
    private readonly string player = "Player";
    private PlayerInput playerInput;
    private FrameInput frameInput;

    private bool doorOpen = false;

    private Animator animator;

    private void Awake() {
        playerInput = GameObject.Find(player).GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
    }

    private void Update() {
        GatherInput();
        Interaction(topDoor.position, "OpenTop");
        Interaction(botDoor.position, "OpenBot");
    }

    private void GatherInput()
    {
        frameInput = playerInput.FrameInput;
    }

    private void Interaction(Vector2 point, String openAnimation)
    {
        if (!DoorHitbox(point)) { return; }
        
        if (frameInput.Interact) {
            doorOpen = !doorOpen; 

            if (doorOpen) { animator.Play(openAnimation); } 
            else { animator.Play("Close"); }
        }
    }

    private bool DoorHitbox(Vector2 point)
    {
        bool isTouching = Physics2D.OverlapBox(point, doorHitbox, 0, playerLayer);
        return isTouching;
    }

    // TODO: Remove after use

    #if UNITY_EDITOR

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(topDoor.position, doorHitbox);
        Gizmos.DrawWireCube(botDoor.position, doorHitbox);
    }

    #endif
}
