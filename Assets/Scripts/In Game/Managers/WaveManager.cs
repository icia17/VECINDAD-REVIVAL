using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Count")]
    public static float wave = 0;

    [Header("Wolf Spawners and Holder")]
    [SerializeField] List<Transform> spawn;
    [SerializeField] Transform wolfHolder;

    [Header("Wolf Types (Usando ScriptableObjects)")]
    [SerializeField] List<PoolableObjectSO> wolfTypes; // <--- CAMBIO PRINCIPAL

    [Header("Wave Specific Wolf Amount")]
    [SerializeField] int wolfAmount = 15;

    [Header("Max Wolves Present at Once")]
    [SerializeField] int maxAmount = 5;

    [Header("Wave Specific Wolf 1/X Spawn Chance")]
    [SerializeField] List<int> percent;

    [Header("Timer Countdown")]
    [SerializeField] float timerCD;

    [Header("Current State Text Object")]
    [SerializeField] TextMeshProUGUI stateObject;

    [Header("Lightbulbs Game Object")]
    [SerializeField] Animator lights;

    [Header("Enable/Disable Objects")]
    [SerializeField] GameObject weaponStoreButton;
    [SerializeField] GameObject weaponStoreParent;
    [SerializeField] GameObject buildStoreButton;
    [SerializeField] GameObject buildStoreParent;
    [SerializeField] GameObject meleeStoreButton;
    [SerializeField] GameObject meleeStoreParent;
    [SerializeField] GameObject hud;
    [SerializeField] GameObject tutorial;

    [Header("Wave Tracking - Debug Info")]
    [SerializeField] int wolvesAlive = 0;   
    
    public static int wolvesLeft;
    public static WaveManager Instance; 
    
    float baseTimerCD;
    int wolfCount;
    int wolvesToSpawn;

    private void Awake()
    {
        Instance = this; 
    }

    private void Start()
    {
        GameManager.State = GameState.Timer;
        baseTimerCD = timerCD;
        wolvesToSpawn = wolfAmount;
        wolvesLeft = wolfAmount;
        wolvesAlive = 0;
        wave = 0;
    }

    private void Update()
    {
        wolfCount = 0;
        foreach (Transform child in wolfHolder)
        {
            if (child.gameObject.activeSelf)
            {
                wolfCount++;
            }
        }

        switch (GameManager.State)
        {
            case GameState.Timer:
                Timer();
                break;
            case GameState.Wave:
                Wave();
                break;
            case GameState.Lose:
                Lose();
                break;
        }
    }
    
    public void OnWolfDeath()
    {
        wolvesAlive--;
        wolvesLeft--;
        
        Logger.Log($"Wolf died! Wolves alive: {wolvesAlive}, Wolves to spawn: {wolvesToSpawn}");
        
        CheckWaveCompletion();
    }

    private void CheckWaveCompletion()
    {
        if (wolvesToSpawn <= 0 && wolvesAlive <= 0)
        {
            Logger.Log($"Wave {wave} completed! All wolves spawned and defeated.");
            CompleteWave();
        }
    }

    private void CompleteWave()
    {
        NextWaveBuffs();
        GameManager.State = GameState.Timer;
        meleeStoreButton.SetActive(true);
        weaponStoreButton.SetActive(true);
        buildStoreButton.SetActive(true);
        DestroyEmptyTurrets();

        AudioManager.Instance.audioMixer.SetFloat("lowpass", 500);
        lights.Play("On");
    }

    private void Timer()
    {
        timerCD -= Time.deltaTime;
        stateObject.text = "Intermisión: " + Mathf.RoundToInt(timerCD) + "s";

        if (Input.GetMouseButtonDown(1))
        {
            timerCD = 0;
        }

        if (timerCD <= 0)
        {
            StartNewWave();
        }
    }

    private void StartNewWave()
    {
        tutorial.SetActive(false);
        wave++;
        
        wolvesToSpawn = wolfAmount;
        wolvesAlive = 0;
        wolvesLeft = wolfAmount; 
        
        GameManager.State = GameState.Wave;
        timerCD = baseTimerCD;
        GameManager.inStore = false;

        if (buildStoreParent.activeSelf || weaponStoreParent.activeSelf || meleeStoreParent.activeSelf)
        {
            meleeStoreParent.SetActive(false);
            weaponStoreParent.SetActive(false);
            buildStoreParent.SetActive(false);
            hud.SetActive(true);
        }

        meleeStoreButton.SetActive(false);
        weaponStoreButton.SetActive(false);
        buildStoreButton.SetActive(false);

        if (!AudioManager.Instance.musicSource.isPlaying)
        {
            string index = Random.Range(0, 2).ToString();
            AudioManager.Instance.PlayMusic(index);
        }

        AudioManager.Instance.audioMixer.SetFloat("lowpass", 5000);
        lights.Play("Off");
        
        Logger.Log($"Starting wave {wave} - Wolves to spawn: {wolvesToSpawn}");
    }

    private void Wave()
    {
        stateObject.text = "Oleada " + wave;

        Vector2 chosenSpawn = spawn[Random.Range(0, spawn.Count)].transform.position;

        if (wolfCount < maxAmount && wolvesToSpawn > 0)
        {
            for (int i = 0; i < wolfTypes.Count; i++)
            {
                if (Random.Range(1, percent[i] + 1) == 1 && wolvesToSpawn > 0)
                {
                    wolvesToSpawn--;
                    wolvesAlive++; 

                    PoolableObjectSO wolfToSpawn = wolfTypes[i];
                    GameObject wolfInstance = ObjectPooler.Instance.SpawnFromPool(wolfToSpawn, chosenSpawn, Quaternion.identity);
                    
                    Logger.Log($"SPAWNING A WOLF! Remaining to spawn: {wolvesToSpawn}, Currently alive: {wolvesAlive}");
                    
                    if (wolfInstance != null)
                    {
                        wolfInstance.transform.SetParent(wolfHolder);
                        
                        WolfHealthController healthController = wolfInstance.GetComponent<WolfHealthController>();
                        if (healthController != null)
                        {
                            healthController.poolableType = wolfToSpawn;
                            healthController.InitializeWolf(chosenSpawn);
                        }
                    }
                }
            }
        }
        
        CheckWaveCompletion();
    }

    private void DestroyEmptyTurrets()
    {
        TurretRangedController[] turrets = FindObjectsOfType<TurretRangedController>();

        foreach (var turret in turrets)
        {
            if (turret.empty)
            {
                Destroy(turret.transform.parent.gameObject);
            }
        }
    }

    private void NextWaveBuffs()
    {
        wolfAmount += 1;
        wolvesLeft = wolfAmount;

        for (int i = 0; i < percent.Count; i++)
        {
            if (percent[i] == 1) { continue; }
            percent[i] -= 1;
        }

        if ((wave + 1) % 3 == 0)
        {
            maxAmount++;
        }
    }

    private void Lose()
    {
        AudioManager.Instance.musicSource.volume = 0.5f;
    }
}