using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    ParticleSystem partSys;
    PlayerList playerList;
    ChosenPlayerChanger chosenPlayerChanger;
    Rigidbody2D rb;
    Animator animator;
    AudioSource audioSource;

    [Header("Player Health / Health Limit")]
    public float health = 100;
    public float healthLimit = 105;
    public bool alive = true;

    [Header("Blood Loss Cooldown and Amount")]
    public float bloodLossCD;
    public float bloodLossAmount;

    [Header("On BloodLust Event")]
    public UnityEvent OnBlood;

    [Header("Take Damage SFX")]
    [SerializeField] AudioClip damagedSFX;

    [HideInInspector]
    public bool bloodLust = false;

    float baseBloodLossCD;
    
    private void Start() {
        partSys = GetComponent<ParticleSystem>();   
        playerList = GetComponent<PlayerList>(); 
        chosenPlayerChanger = GetComponentInParent<ChosenPlayerChanger>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        baseBloodLossCD = bloodLossCD;
    }

    public void TakeDamage(float damage)
    {
        audioSource.PlayOneShot(damagedSFX);
        
        partSys.Play();

        health -= damage;
        if (health <= 0 && alive)
        {
            alive = false;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            animator.Play("Death");
        }
    }

    public void Die()
    {
        Destroy(gameObject);
        PlayerListLists.players.Remove(playerList);
        StartCoroutine(chosenPlayerChanger.Change());
    }


    private void Update() {
        if (health > healthLimit) {
            health = healthLimit;
        }

        if (health > 100 && !bloodLust) {
            OnBlood?.Invoke();

            bloodLust = true;
        } else if (health <= 100 && bloodLust) {
            bloodLust = false;
        }

        if (bloodLust) {
            BloodLoss();
        }

        if (GameManager.State == GameState.Timer) {
            health = 100;
        }
    }

    private void BloodLoss()
    {
        if (bloodLossCD > 0) {
            bloodLossCD -= Time.deltaTime;
        } else {
            health -= bloodLossAmount;
            bloodLossCD = baseBloodLossCD;
        }
    }
}