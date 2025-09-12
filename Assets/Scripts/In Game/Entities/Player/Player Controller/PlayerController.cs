using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    FrameInput frameInput;
    PlayerInput playerInput;
    MeleeController meleeController;
    RangedController rangedController;
    ChosenPlayerChanger chosenPlayerChanger;
    InteractableReactions interactables;
    PlayerInventory playerInventory;
    
    [Header("Player Attributes")]
    public float moveSpeed;
    public float bloodlustBonus = 1.25f;

    [Header("Player Arm Activation Check")]
    [SerializeField] GameObject arm;

    [Header("Player Hands Activation Check")]
    [SerializeField] GameObject hands;
    
    [HideInInspector] public bool isMoving = false;

    [Header("Player Properties")]
    public bool isChosen = false;

    [Header("Pause Menu Control")]
    public PauseMenuController pauseMenuController;

    [Header("HUD GameObject")]
    [SerializeField] GameObject HUD;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public bool changingPlayers = false;
    [HideInInspector] public GrabPosition grabPlayer;
    [HideInInspector] public PlayerHealth playerHealth;
    [HideInInspector] public bool swinging = false;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        pauseMenuController = GameObject.Find("Pause Menu").gameObject.GetComponent<PauseMenuController>();    
        grabPlayer = GetComponent<GrabPosition>();
        meleeController = GetComponentInChildren<MeleeController>();
        rangedController = GetComponentInChildren<RangedController>();
        chosenPlayerChanger = GetComponentInParent<ChosenPlayerChanger>();
        playerHealth = GetComponent<PlayerHealth>();
        interactables = GetComponent<InteractableReactions>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        if (!isChosen || GameManager.inStore) { return; }

        Pause();

        if (changingPlayers) { return; }

        Change();

        if (grabPlayer.IsGrabbed()) { return; }
        
        GatherInput();
        OnReload();
        Move();
        Throw();

        if (GameManager.State == GameState.Timer) return;
        
        OnSwing();
        OnShoot();
    }

    private void GatherInput()
    {
        frameInput = playerInput.FrameInput;
    }

    private void OnSwing() {
        if (frameInput.Fire && hands.activeSelf) {
            meleeController.Swing();
        }
    }

    private void OnShoot() {
        if (frameInput.Fire && arm.activeSelf) {
            rangedController.Shoot();
        }
    }

    private void OnReload() {
        if (frameInput.Reload && arm.activeSelf) {

            if (interactables.nearAmmoBox && !rangedController.currentWeapon.reloading) {
                rangedController.currentWeapon.totalAmmo = rangedController.currentWeapon.unchangedTotalAmmo;
                interactables.currentAmmoBox.Use();
            }

            rangedController.Reload();
        }
    }
    
    private void Move()
    {
        if (frameInput.Move == Vector2.zero) {
            isMoving = false;
            rb.velocity = Vector2.zero;
            
            return; 
        }
        
        isMoving = true;
        rb.velocity = frameInput.Move * moveSpeed * BloodLust() * Swinging();
    }

    private void Throw() {
        if (frameInput.Throw && !swinging) {
            playerInventory.ThrowWeapon();
        }
    }

    private float BloodLust()
    {
        if (playerHealth.bloodLust) {
            return bloodlustBonus;
        } else {
            return 1.0f;
        }
    }

    private float Swinging()
    {
        if (meleeController.swinging) {
            return 0.5f;
        } else {
            return 1.0f;
        }
    }

    private void Pause() {
        if (!frameInput.Pause) { return; }
        
        if (pauseMenuController.isPaused) {
            AudioManager.Instance.audioMixer.SetFloat("lowpass", 5000);

            HUD.SetActive(true);

            pauseMenuController.EscToGame();
            return;
        }

        AudioManager.Instance.audioMixer.SetFloat("lowpass", 500);

        HUD.SetActive(false);
        
        pauseMenuController.OpenPauseMenu();
    }

    private void Change() {
        if (frameInput.Change || grabPlayer.IsGrabbed()) {
            StartCoroutine(chosenPlayerChanger.Change());
        }
    }
}
