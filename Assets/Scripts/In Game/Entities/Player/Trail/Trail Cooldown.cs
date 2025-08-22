using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCooldown : MonoBehaviour
{
    [Header("Ghost Trail Properties")]
    [SerializeField] float trailCD;
    [SerializeField] GameObject trailObject;
    [SerializeField] GameObject trailHolder;
    [SerializeField] Sprite playerSprite;
    [SerializeField] Transform headTransform;

    PlayerHealth playerHealth;
    float baseTrailCD;

    private void Start() {
        playerHealth = GetComponent<PlayerHealth>();

        baseTrailCD = trailCD;
    }

    private void Update() {
        if (playerHealth.bloodLust) {
            Trail();
        }
    }

    private void Trail()
    {
        trailCD -= Time.deltaTime;

        if (trailCD <= 0) {
            trailCD = baseTrailCD;

            GameObject trail = Instantiate(trailObject, transform.position, headTransform.rotation, trailHolder.transform);

            trail.GetComponent<GhostTrail>().Init(playerSprite);
        }
    }
}
