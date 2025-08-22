using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAnimatorController : MonoBehaviour
{
    [Header("Player Body Animator")]
    [SerializeField] Animator animator;
    private PlayerController playerController;
    private bool moving;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        if (!playerController.isChosen) { return; }

        GetState();
        SetAnimation();
    }

    private void GetState()
    {
        moving = playerController.isMoving;
    }

    private void SetAnimation()
    {
        animator.SetBool("walk", moving);
    }
}
