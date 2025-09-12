using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RedWalkToTarget : MonoBehaviour
{
    [Header("Wolf lower body animator")]
    public Animator animator;

    ClosestTarget closestTarget;
    NavMeshAgent agent;
    Vector3 posVelocity = Vector3.zero;
    IGrabber grabPlayer;
    GameObject[] exits;
    
    [HideInInspector]
    public GameObject closestExit;

    [HideInInspector]
    public bool goingToExit = false;

    private bool stopWalking = false;
    
    private void Start() {
        GetComponent<WolfHealthController>().OnDeath.AddListener(() => { stopWalking = true; });
        closestTarget = GetComponent<ClosestTarget>();
        agent = GetComponent<NavMeshAgent>();
        
        grabPlayer = GetComponentInChildren<IGrabber>();

        exits = GameObject.FindGameObjectsWithTag("Exit");
        closestExit = exits[0];

        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update() {
        if (closestTarget.closestPlayer == null) return;
        
        if (stopWalking) {
            animator.Play("Idle");
            return; 
        }
        
        animator.Play("Walk");

        if (grabPlayer.IsGrabbing()) {
            agent.SetDestination(closestExit.transform.position);

            goingToExit = true;
        } else {
            agent.SetDestination(closestTarget.closestPlayer.transform.position);
        }

        transform.position = Vector3.SmoothDamp(transform.position, agent.nextPosition, ref posVelocity, 0.1f);
    }

    private void FixedUpdate() {
        foreach(var exit in exits) {
            if (Vector2.Distance(transform.position, exit.transform.position) < Vector2.Distance(transform.position, closestExit.transform.position)) {
                closestExit = exit;
            }
        }
    }
}