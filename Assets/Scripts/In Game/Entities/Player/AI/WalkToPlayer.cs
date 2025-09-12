using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkToPlayer : MonoBehaviour
{
    [Header("Body Animator")]
    [SerializeField] Animator animator;

    [Header("Bloodlust Bonus Speed")]
    [SerializeField] float bloodlustSpeed;
    
    PlayerController player;
    PlayerHealth health;
    NavMeshAgent agent;
    ChosenPlayerChanger chosenPlayerChanger;
    IGrabbable grabPlayer;

    float baseSpeed;

    private void Start() {
        player = GetComponent<PlayerController>();
        health = GetComponent<PlayerHealth>();
        agent = GetComponent<NavMeshAgent>();
        chosenPlayerChanger = GetComponentInParent<ChosenPlayerChanger>();
        grabPlayer = GetComponent<IGrabbable>();
        
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        baseSpeed = agent.speed;
    }

    private void LateUpdate() {
        if (PlayerListLists.chosenPlayer == null || PlayerListLists.players.Count <= 1 || !health.alive) { return; }
        
        Vector2 chosenPos = PlayerListLists.chosenPlayer.transform.position;
        
        ChangeDestination(chosenPos);

        agent.speed = BloodLust();

        if (player.isChosen || PlayerListLists.chosenPlayer == null) { return; }
        
        ChangeAnimation(chosenPos);
    }

    private float BloodLust()
    {
        if (health.bloodLust) {
            return bloodlustSpeed;
        } else {
            return baseSpeed;
        }
    }

    private void ChangeDestination(Vector2 chosenPos) {
        if (player.isChosen) {
            agent.nextPosition = transform.position;
            agent.SetDestination(chosenPlayerChanger.nextPlayer.transform.position);
        } else {
            agent.SetDestination(chosenPos);

            if (!grabPlayer.IsGrabbed()) 
                transform.position = Vector2.Lerp(transform.position, agent.nextPosition, 0.1f);
            else
                agent.SetDestination(transform.position);
        }
    }

    private void ChangeAnimation(Vector2 chosenPos) {
        if (health.health <= 0) {
            animator.SetBool("walk", false);
            return;
        }

        if (Vector2.Distance(chosenPos, transform.position) <= agent.stoppingDistance) {
            animator.SetBool("walk", false);
        } else {
            animator.SetBool("walk", true);
        }
    }
}