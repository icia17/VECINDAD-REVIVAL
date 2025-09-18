using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkToTarget : MonoBehaviour
{
    [Header("Wolf lower body animator")]
    public Animator animator;

    ClosestTarget closestTarget;
    NavMeshAgent agent;
    Vector3 posVelocity = Vector3.zero;
    WolfLifeController wolfLife;

    private void Start() {
        wolfLife = GetComponent<WolfLifeController>();
        closestTarget = GetComponent<ClosestTarget>();
        agent = GetComponent<NavMeshAgent>();
        
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void OnEnable()
    {
        agent?.ResetPath();
    }

    private void FixedUpdate() {
        if (wolfLife.isDead) {
            animator.Play("Idle");
            return; 
        }
        
        animator.Play("Walk");
        
        if (closestTarget.closestPlayer == null) { return; }
        
        agent.SetDestination(closestTarget.closestPlayer.transform.position);

        transform.position = Vector3.SmoothDamp(transform.position, agent.nextPosition, ref posVelocity, 0.1f);
    }
}