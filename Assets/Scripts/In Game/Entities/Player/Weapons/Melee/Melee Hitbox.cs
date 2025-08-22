using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    PlayerHealth playerHealth;
    MeleeController melee;

    private void Start() {
        melee = GetComponentInParent<MeleeController>();
        playerHealth = GetComponentInParent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var wolfLife = other.GetComponentInParent<WolfLifeController>();
        
        melee.currentWeapon.uses--;
        
        playerHealth.health += melee.currentWeapon.damage/20;

        wolfLife.TakeSplatDamage(melee.currentWeapon.damage);
    }
}
