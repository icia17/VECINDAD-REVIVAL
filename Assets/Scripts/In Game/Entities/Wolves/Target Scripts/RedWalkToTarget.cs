using UnityEngine;
using UnityEngine.AI;

public class RedWalkToTarget : MonoBehaviour
{
    [Header("Wolf lower body animator")]
    public Animator animator;

    private ClosestTarget closestTarget;
    private NavMeshAgent agent;
    private Vector3 posVelocity = Vector3.zero;
    private IGrabber grabPlayer;
    private GameObject[] exits;
    
    [HideInInspector]
    public GameObject closestExit;

    [HideInInspector]
    public bool goingToExit = false;

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
        grabPlayer = GetComponentInChildren<IGrabber>();

        exits = GameObject.FindGameObjectsWithTag("Exit");
        closestExit = exits != null && exits.Length > 0 ? exits[0] : null;
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
        agent?.ResetPath();
    }
    
    private void Update() 
    {
        if (closestTarget == null || closestTarget.closestPlayer == null) return;
        
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

        UpdateDestination();
        UpdatePosition();
    }

    private void UpdateDestination()
    {
        if (agent == null) return;

        if (grabPlayer != null && grabPlayer.IsGrabbing()) 
        {
            if (closestExit != null)
            {
                agent.SetDestination(closestExit.transform.position);
            }
            goingToExit = true;
        } 
        else 
        {
            if (closestTarget.closestPlayer != null)
            {
                agent.SetDestination(closestTarget.closestPlayer.transform.position);
            }
            goingToExit = false;
        }
    }

    private void UpdatePosition()
    {
        if (agent == null) return;

        transform.position = Vector3.SmoothDamp(
            transform.position, 
            agent.nextPosition, 
            ref posVelocity, 
            SMOOTH_TIME
        );
    }

    private void FixedUpdate() 
    {
        if (exits == null || exits.Length == 0) return;

        FindClosestExit();
    }

    private void FindClosestExit()
    {
        float closestDistance = closestExit != null 
            ? Vector2.Distance(transform.position, closestExit.transform.position) 
            : float.MaxValue;

        foreach (var exit in exits) 
        {
            if (exit == null) continue;

            float distance = Vector2.Distance(transform.position, exit.transform.position);
            
            if (distance < closestDistance) 
            {
                closestDistance = distance;
                closestExit = exit;
            }
        }
    }
}