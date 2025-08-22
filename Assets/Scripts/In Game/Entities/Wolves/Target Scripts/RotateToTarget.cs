using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RotateToTarget : MonoBehaviour
{
    [Header("Rotation Time")]
    public float rotationTime;
    NavMeshAgent agent;

    private void Start() {
        agent = GetComponentInParent<NavMeshAgent>();
    }

    private void FixedUpdate() {
        Vector3 velocity = agent.velocity;

        if (velocity.sqrMagnitude > 0.01f) {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle - 90)), rotationTime * Time.deltaTime);
        }
    }
}
