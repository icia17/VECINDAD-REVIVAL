using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleCollision : MonoBehaviour
{
    [SerializeField] private ParticleCollisionConfigSO particleColConfig;
    
    private PoolableObjectSO poolableSplat;
    
    private float randomSplatChance = 0.01f;
    private float splatDetectionDuration = 0.1f;
    private float maxSplatDistance = 3.5f; 
    
    private ParticleSystem particle;
    private Transform splatHolder;
    
    private Vector3 cachedPosition;
    private float maxSplatDistanceSqr; 
    
    private ParticleSystem.Particle[] particleArray;
    private List<ParticleCollisionEvent> collisionEventsList = new List<ParticleCollisionEvent>();
    
    private float splatTimer = 0f;
    private bool isSplatting = false;
    private Coroutine currentSplatCoroutine;

    private void Awake()
    {
        if (particleColConfig == null)
        {
            Logger.LogError("particleColConfig is null! Assign it in the Inspector. - From: " + gameObject.name);
            return;
        }
    
        if (particleColConfig.splatPrefab == null)
        {
            Logger.LogError("splatPrefab in particleColConfig is null! Set it in the ScriptableObject asset. - From: " + gameObject.name);
            return;
        }
        
        poolableSplat = particleColConfig.splatPrefab;
        randomSplatChance = particleColConfig.randomSplatChance;
        splatDetectionDuration = particleColConfig.splatDetectionDuration;
        maxSplatDistance = particleColConfig.maxSplatDistance;
    }

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        splatHolder = GameObject.FindWithTag("SplatHolder").transform;
        
        particleArray = new ParticleSystem.Particle[particle.main.maxParticles];
        
        maxSplatDistanceSqr = maxSplatDistance * maxSplatDistance;
    }

    public void BeginSplat()
    {
        if (particle == null || particleArray == null || isSplatting) 
        {
            return;
        }
        
        currentSplatCoroutine = StartCoroutine(SplatTime());
    }

    public void EndSplat()
    {
        if (currentSplatCoroutine != null)
        {
            StopCoroutine(currentSplatCoroutine);
            currentSplatCoroutine = null;
        }
        
        isSplatting = false;
        splatTimer = 0f;
        
        collisionEventsList.Clear();
        
        cachedPosition = Vector3.zero;
    }

    private IEnumerator SplatTime()
    {
        isSplatting = true;
        splatTimer = 0f;
        int currentParticleIndex = 0;
    
        cachedPosition = transform.position;

        while (splatTimer < splatDetectionDuration)
        {
            int numParticlesAlive = particle.GetParticles(particleArray);
            
            if (numParticlesAlive > 0 && currentParticleIndex < numParticlesAlive)
            {
                Vector3 particlePos = particleArray[currentParticleIndex].position;
                float distanceSqr = (particlePos - cachedPosition).sqrMagnitude;
            
                if (distanceSqr <= maxSplatDistanceSqr && Random.value <= randomSplatChance)
                {
                    CreateSplat(particlePos, true);
                }
            }
            
            currentParticleIndex++;
            
            if (currentParticleIndex >= numParticlesAlive)
            {
                currentParticleIndex = 0;
            }
    
            splatTimer += Time.deltaTime;
            yield return null;
        }
    
        isSplatting = false;
        currentSplatCoroutine = null;
    }
    
    private void CreateSplat(Vector3 position, bool inside)
    {
        GameObject splat = ObjectPooler.Instance.SpawnFromPool(poolableSplat, position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));

        var splatResizer = splat.GetComponent<DestroyAfter>();
        
        splatResizer.Init(poolableSplat);
        
        var splatRend = splat.GetComponent<SpriteRenderer>();
        
        if (inside)
        {
            splatRend.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            splatRend.sortingOrder = -999;
        }
        else
        {
            splatRend.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            splatRend.sortingOrder = 15000;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (splatTimer < splatDetectionDuration)
        {
            collisionEventsList.Clear();
            
            ParticlePhysicsExtensions.GetCollisionEvents(particle, other, collisionEventsList);
            
            cachedPosition = transform.position;

            for (int i = 0; i < collisionEventsList.Count; i++)
            {
                Vector3 intersection = collisionEventsList[i].intersection;
                
                float distanceSqr = (intersection - cachedPosition).sqrMagnitude;
                
                if (distanceSqr <= maxSplatDistanceSqr)
                {
                    CreateSplat(intersection, false);
                }
            }
        }
    }
}