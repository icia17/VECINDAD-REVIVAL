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
    [SerializeField] List<PoolableObjectSO> wolfTypes;

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

    [Header("Wave Tracking - Debug Info")]
    [SerializeField] int wolvesAlive = 0;   
    
    public static int wolvesLeft;
    public static WaveManager Instance;

    // Tutorial stuff
    public static bool finishTutorial = false;
    
    float baseTimerCD;
    int wolvesToSpawn;
    
    // Cache for performance
    private int cachedWolfCount = 0;
    private float wolfCountUpdateInterval = 0.1f; // Update every 0.1 seconds instead of every frame
    private float wolfCountTimer = 0f;
    private WaitForSeconds spawnDelay;
    private Coroutine spawnCoroutine;
    
    // Text update optimization
    private string lastStateText = "";
    private int lastTimerValue = -1;

    private void Awake()
    {
        Instance = this;
        spawnDelay = new WaitForSeconds(0.05f); // Small delay between spawns
    }

    private void Start()
    {
        if (!finishTutorial)
        {
            GameManager.State = GameState.Tutorial;
        }
        else
        {
            GameManager.State = GameState.Timer;
        }

        baseTimerCD = timerCD;
        wolvesToSpawn = wolfAmount;
        wolvesLeft = wolfAmount;
        wolvesAlive = 0;
        wave = 0;
    }

    private void Update()
    {
        // Only update wolf count periodically instead of every frame
        wolfCountTimer += Time.deltaTime;
        if (wolfCountTimer >= wolfCountUpdateInterval)
        {
            wolfCountTimer = 0f;
            UpdateWolfCount();
        }

        switch (GameManager.State)
        {
            case GameState.Timer:
                Timer();
                break;
            case GameState.Wave:
                // Wave spawning now handled by coroutine
                UpdateWaveUI();
                break;
            case GameState.Lose:
                Lose();
                break;
        }
    }
    
    // Optimized wolf count - only counts when needed
    private void UpdateWolfCount()
    {
        cachedWolfCount = 0;
        foreach (Transform child in wolfHolder)
        {
            if (child.gameObject.activeSelf)
            {
                cachedWolfCount++;
            }
        }
    }
    
    public void OnWolfDeath()
    {
        wolvesAlive--;
        wolvesLeft--;
        cachedWolfCount--; // Update cache immediately
        
        Logger.Log($"Wolf died! Wolves alive: {wolvesAlive}, Wolves to spawn: {wolvesToSpawn}");
        
        CheckWaveCompletion();
    }
    
    public void StartTimerCountdownAfterTutorial()
    {
        finishTutorial = true;
        GameManager.State = GameState.Timer;
        timerCD = baseTimerCD;
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
        // Stop spawn coroutine if running
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        
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
        
        // Only update text when the value actually changes
        int roundedTime = Mathf.RoundToInt(timerCD);
        if (roundedTime != lastTimerValue)
        {
            lastTimerValue = roundedTime;
            stateObject.text = "Intermisión: " + roundedTime + "s";
        }

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
        wave++;
        
        wolvesToSpawn = wolfAmount;
        wolvesAlive = 0;
        wolvesLeft = wolfAmount;
        cachedWolfCount = 0; // Reset cache
        
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
        
        // Start spawn coroutine instead of spawning in Update
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnWolvesCoroutine());
    }
    
    private void UpdateWaveUI()
    {
        string newText = "Oleada " + wave;
        if (newText != lastStateText)
        {
            lastStateText = newText;
            stateObject.text = newText;
        }
    }
    
    // Spawn wolves using coroutine to spread load over frames
    private IEnumerator SpawnWolvesCoroutine()
    {
        while (wolvesToSpawn > 0)
        {
            if (cachedWolfCount < maxAmount)
            {
                Vector2 chosenSpawn = spawn[Random.Range(0, spawn.Count)].position;
                
                // Try to spawn a wolf based on chances
                for (int i = 0; i < wolfTypes.Count; i++)
                {
                    if (Random.Range(1, percent[i] + 1) == 1 && wolvesToSpawn > 0)
                    {
                        wolvesToSpawn--;
                        wolvesAlive++;
                        cachedWolfCount++; // Update cache immediately

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
                        
                        // Small delay between spawns to spread load
                        yield return spawnDelay;
                        break; // Only spawn one wolf per iteration
                    }
                }
            }
            else
            {
                // Wait before checking again if at max capacity
                yield return spawnDelay;
            }
        }
        
        spawnCoroutine = null;
        CheckWaveCompletion();
    }

    private void DestroyEmptyTurrets()
    {
        // Cache the array to avoid repeated FindObjectsOfType calls
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
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        
        AudioManager.Instance.musicSource.volume = 0.5f;
    }
}