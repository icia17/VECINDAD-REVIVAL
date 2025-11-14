using UnityEngine;
using UnityEngine.AI;

public class RotateToTarget : MonoBehaviour
{
    [Header("Rotation Time")]
    public float rotationTime = 5f;
    
    private NavMeshAgent agent;
    private const float VELOCITY_THRESHOLD = 0.01f;
    private const float ROTATION_OFFSET = -90f;

    private void Start() 
    {
        agent = GetComponentInParent<NavMeshAgent>();
    }

    private void FixedUpdate() 
    {
        if (agent == null) return;

        Vector3 velocity = agent.velocity;

        if (velocity.sqrMagnitude > VELOCITY_THRESHOLD) 
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + ROTATION_OFFSET);
            
            transform.rotation = Quaternion.Lerp(
                transform.rotation, 
                targetRotation, 
                rotationTime * Time.deltaTime
            );
        }
    }
}