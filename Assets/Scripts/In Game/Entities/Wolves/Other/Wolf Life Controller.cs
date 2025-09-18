using UnityEngine;
using UnityEngine.Events;

public class WolfLifeController : MonoBehaviour
{
    
    [Header("Wolf life points")]
    public float maxWolfLife;
    public float currentWolfLife;

    [Header("Wolf full body animator")]
    public Animator animator;

    [Header("Death unity event")]
    public UnityEvent OnDeath;

    [HideInInspector]
    public bool isDead = false;
    
    [HideInInspector]
    public PoolableObjectSO poolableType;

    ParticleSystem particles;

    
    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        if (maxWolfLife <= 0)
        {
            maxWolfLife = 100;
        }
    }

    public void InitializeWolf()
    {
        currentWolfLife = maxWolfLife;
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentWolfLife -= damage;
        animator.Play("Damaged");
    }

    private void Update()
    {
        if (currentWolfLife <= 0 && !isDead)
        {
            isDead = true;
            WaveManager.wolvesLeft--;
            OnDeath?.Invoke();
            animator.Play("Death");
        }
    }
    public void TakeSplatDamage(float damage)
    {
        currentWolfLife -= damage;
        particles.Play();
        animator.Play("Damaged");
    }

    
    public void PostMortem()
    {
        ObjectPooler.Instance.ReturnToPool(poolableType, this.gameObject);
    }
}