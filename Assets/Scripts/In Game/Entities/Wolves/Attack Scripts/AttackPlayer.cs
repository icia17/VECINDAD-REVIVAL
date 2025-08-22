using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AttackPlayer : MonoBehaviour
{
    [Header("Player layer")]
    public LayerMask layer;

    [Header("Attack hitbox stats")]
    public Transform attackPos;
    public float attackRadius;

    [Header("Attack damage")]
    public float damage;

    Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();    
    }

    private void Update() {
        if (Attack()) {
            animator.Play("Attack");
        }
    }
    
    private bool Attack() {
        var isColliding = Physics2D.OverlapCircle(attackPos.position, attackRadius, layer);
        return isColliding;
    }

    private void Hit() {
        // Get all colliders within the specified radius that are on the "Player" layer
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPos.position, attackRadius, layer);

        foreach (Collider2D player in hitPlayers) {
            // Apply damage to each player
            player.GetComponentInParent<PlayerHealth>().TakeDamage(damage);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }
    
}
