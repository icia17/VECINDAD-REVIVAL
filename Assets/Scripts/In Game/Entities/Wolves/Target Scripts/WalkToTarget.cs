using UnityEngine;
using UnityEngine.AI;

public class WalkToTarget : MonoBehaviour
{
    [Header("Wolf lower body animator")]
    public Animator animator;

    private ClosestTarget closestTarget;
    private NavMeshAgent agent;
    private Vector3 posVelocity = Vector3.zero;
    private bool stopWalking = false;
    
    private const float SMOOTH_TIME = 0.1f;

    private void Start() 
    {
        CacheComponents();
        ConfigureNavMeshAgent();
    }

    private void CacheComponents()
    {
        var healthController = GetComponent<WolfHealthController>();
        if (healthController != null)
        {
            healthController.OnDeath.AddListener(() => { stopWalking = true; });
        }

        closestTarget = GetComponent<ClosestTarget>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void ConfigureNavMeshAgent()
    {
        if (agent == null) return;

        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void OnEnable()
    {
        stopWalking = false;
    }

    private void FixedUpdate() 
    {
        if (stopWalking) 
        {
            if (animator != null)
            {
                animator.Play("Idle");
            }
            return; 
        }
        
        if (animator != null)
        {
            animator.Play("Walk");
        }
        
        if (closestTarget == null || closestTarget.closestPlayer == null || agent == null) 
        {
            return;
        }
        
        agent.SetDestination(closestTarget.closestPlayer.transform.position);

        transform.position = Vector3.SmoothDamp(
            transform.position, 
            agent.nextPosition, 
            ref posVelocity, 
            SMOOTH_TIME
        );
    }
}