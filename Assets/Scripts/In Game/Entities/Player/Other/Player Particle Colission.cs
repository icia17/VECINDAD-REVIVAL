using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerParticleCollision : MonoBehaviour
{
    ParticleSystem particle;
    public GameObject splatPrefab;
    private Transform splatHolder;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    public AudioSource audioSource;
    public AudioClip[] sounds;
    public float SoundCapResetSpeed = 0.55f;
    public int MaxSounds = 3;
    float TimePassed;
    int soundsPlayed;
    private bool hasSplattered = false;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        splatHolder = GameObject.Find("Splat Holder").transform;
    }

    private void Update()
    {
        TimePassed += Time.deltaTime;
        if (TimePassed > SoundCapResetSpeed)
        {
            soundsPlayed = 0;
            TimePassed = 0;
        }

        if (!hasSplattered) {
            hasSplattered = true;
            StartCoroutine(GroundSplatter());
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particle, other, collisionEvents);

        int count = collisionEvents.Count;

        for (int i = 0; i < count; i++)
        {
            Instantiate(splatPrefab, collisionEvents[i].intersection, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)), splatHolder);
            
            /* if (soundsPlayed < MaxSounds)
            {
                soundsPlayed += 1;
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)], Random.Range(0.1f, 0.35f));
            }
            */ 
        }
    }

    private IEnumerator GroundSplatter() {
        float wait = Random.Range(0.1f,0.2f);

        yield return new WaitForSeconds(wait);

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particle.main.maxParticles];
        
        for(int i = 0; i < particle.GetParticles(particles); i++) {
            GameObject splat = Instantiate(splatPrefab, particles[i].position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)), splatHolder);
            var splatRend = splat.GetComponent<SpriteRenderer>();
            splatRend.sortingOrder = -999;
        } 

        yield return new WaitForSeconds(0.5f);

        hasSplattered = false;
    }
}
