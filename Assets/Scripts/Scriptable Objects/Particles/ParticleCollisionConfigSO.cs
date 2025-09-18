using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Particle Configuration/Particle Collision Config")]
public class ParticleCollisionConfigSO : ScriptableObject
{
    [Header("Splat Settings")]
    public GameObject splatPrefab;
    [Range(0f, 0.5f)] public float randomSplatChance = 0.01f;
    [Range(0f, 0.35f)] public float splatDetectionDuration = 0.1f;
    public float maxSplatDistance = 3.5f; 
}