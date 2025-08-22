using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleCollision : MonoBehaviour
{
    ParticleSystem particle;
    public GameObject splatPrefab;
    private Transform splatHolder;
    private BulletStatistics bulletStatistics;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    public float SoundCapResetSpeed = 0.55f;
    public int MaxSounds = 3;
    float TimePassed;
    private bool hasSplattered = false;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        bulletStatistics = GetComponent<BulletStatistics>();
        splatHolder = GameObject.Find("Splat Holder").transform;
    }

    private void Update()
    {
        TimePassed += Time.deltaTime;
        if (TimePassed > SoundCapResetSpeed)
        {
            TimePassed = 0;
        }

        if (bulletStatistics.canSplatter && !hasSplattered) {
            hasSplattered = true;
            StartCoroutine(GroundSplatter());
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!bulletStatistics.canSplatter) {
            Destroy(this);
            return;
        }

        ParticlePhysicsExtensions.GetCollisionEvents(particle, other, collisionEvents);

        int count = collisionEvents.Count;

        for (int i = 0; i < count; i++)
        {
            Instantiate(splatPrefab, collisionEvents[i].intersection, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)), splatHolder);
        }
    }
    
    private IEnumerator GroundSplatter() {
        float wait = Random.Range(0.1f,0.4f);

        yield return new WaitForSeconds(wait);

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particle.main.maxParticles];
        
        for(int i = 0; i < particle.GetParticles(particles); i++) {
            GameObject splat = Instantiate(splatPrefab, particles[i].position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)), splatHolder);
            var splatRend = splat.GetComponent<SpriteRenderer>();
            splatRend.sortingOrder = -999;
        } 
    }
}
