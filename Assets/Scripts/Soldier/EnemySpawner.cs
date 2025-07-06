using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    // Wave definition ¨C currently includes count, whether to spawn a boss, and delay
    [System.Serializable]
    public class Wave
    {
        public int enemyCount;           // Number of regular enemies in this wave
        public bool spawnBoss;           // Whether to spawn a boss
        public float waveDelay;          // Delay between the *end* of this wave and the start of the next
        public float enemySpawnInterval = 1f; // Spawn interval for all enemies
    }

    [Header("General Settings")]
    public Transform spawnPoint; // Spawn point

    [Header("Enemy Settings (Global)")]
    public GameObject enemyPrefab; // Enemy prefab used in all waves

    [Header("Boss Settings (Global)")]
    public GameObject bossPrefab;  // Boss prefab used when spawnBoss is true

    [Header("Wave Configuration")]
    public int initialPreparationTime = 20;
    public Wave[] waves = new Wave[3]; // Configure waves in Inspector

    [Header("Events (Optional)")]
    public UnityEvent OnPreparationStart;
    public UnityEvent OnPreparationEnd;
    public UnityEvent OnWaveStart;
    public UnityEvent OnAllWavesComplete;

    private int currentWaveIndex = 0;

    public TextMeshProUGUI preparationTimeText;
    public GameObject preparationTimeTextObject;
    public Camera mainCam;
    public Image backgroundImage;
    private Color colorA = new Color(0f, 0.667f, 1f, 0.588f);
    private Color colorB = new Color(0.510f, 0.835f, 1f, 0.588f);
    public float colorChangeSpeed = 0.5f;

    private bool startWave = false;
    private GameObject[] currentEnemyCount;
    private float currentSceneTime;
    private float waveDelayTime = 0;

    void Start()
    {
        Debug.Log(backgroundImage.color);
        // Check necessary settings
        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint not assigned!", this);
            enabled = false;
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab not assigned!", this);
            enabled = false;
            return;
        }

        // Boss prefab is optional; only needed if spawnBoss is true
        if (waves == null || waves.Length == 0)
        {
            Debug.LogError("Waves configuration is empty!", this);
            enabled = false;
            return;
        }

        // Automatically start wave manager
        StartCoroutine(WaveManager());
    }

    private void Update()
    {
        string[] enemyTag = { "Enemy", "Tank" };
        currentEnemyCount = FindGameObjectsWithTag(enemyTag);

        // Show preparation time
        if (preparationTimeText != null)
        {
            if (backgroundImage != null && !startWave)
            {
                float t = Mathf.PingPong(Time.time * colorChangeSpeed, 1f);
                Color lerpedColor = Color.Lerp(colorA, colorB, t);
                backgroundImage.color = lerpedColor;
            }
            else
            {
                Debug.LogWarning("backgroundImage is not assigned!");
            }

            Vector3 dirToCamera = mainCam.transform.position - preparationTimeTextObject.transform.position;
            dirToCamera.y = 0; // Keep horizontal
            preparationTimeTextObject.transform.rotation = Quaternion.LookRotation(-dirToCamera);

            int remainingTime = Mathf.CeilToInt(initialPreparationTime - Time.timeSinceLevelLoad);
            preparationTimeText.text = $"Enemy arrive in {remainingTime} second!!";
            if (remainingTime < 1) startWave = true;

            if (startWave)
            {
                if (currentEnemyCount.Length != 0)
                {
                    // Show combat message and flash red
                    Color colorA = new Color(1f, 0f, 0f, 0.588f);
                    Color colorB = new Color(1f, 0.349f, 0.349f, 0.588f);
                    float t = Mathf.PingPong(Time.time * colorChangeSpeed, 1f);
                    Color lerpedColor = Color.Lerp(colorA, colorB, t);
                    backgroundImage.color = lerpedColor;
                    preparationTimeText.text = $"Kill all the enemy!!";
                }
                else
                {
                    // Enemies cleared ¨C show next wave countdown
                    Color colorA = new Color(0f, 0.667f, 1f, 0.588f);
                    Color colorB = new Color(0.510f, 0.835f, 1f, 0.588f);
                    float t = Mathf.PingPong(Time.time * colorChangeSpeed, 1f);
                    Color lerpedColor = Color.Lerp(colorA, colorB, t);
                    int nextWaveTime = Mathf.CeilToInt(currentSceneTime - Time.timeSinceLevelLoad);
                    preparationTimeText.text = $"Enemy arrive in {nextWaveTime} second!!";
                }
            }
        }
    }

    IEnumerator WaveManager()
    {
        Debug.Log("Starting wave spawning...");

        if (initialPreparationTime > 0)
        {
            Debug.Log($"Preparation time before first wave: {initialPreparationTime} seconds...");
            OnPreparationStart?.Invoke();
            yield return new WaitForSeconds(initialPreparationTime);
            Debug.Log("Preparation time over. First wave starting!");
            OnPreparationEnd?.Invoke();
        }

        while (currentWaveIndex < waves.Length)
        {
            Wave currentWave = waves[currentWaveIndex];
            Debug.Log($"Starting wave {currentWaveIndex + 1}/{waves.Length}");

            OnWaveStart?.Invoke();

            // --- Spawn current wave enemies ---

            if (currentWave.enemyCount > 0)
            {
                for (int i = 0; i < currentWave.enemyCount; i++)
                {
                    Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                    GameManager.Instance?.RegisterEnemy();
                    yield return new WaitForSeconds(currentWave.enemySpawnInterval > 0 ? currentWave.enemySpawnInterval : 0.1f);
                }
            }

            if (currentWave.spawnBoss)
            {
                if (bossPrefab != null)
                {
                    Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);
                    GameManager.Instance?.RegisterEnemy();
                    Debug.Log($"Wave {currentWaveIndex + 1}: Boss spawned!");
                }
                else
                {
                    Debug.LogWarning($"Wave {currentWaveIndex + 1} is set to spawn Boss, but Boss prefab is not assigned!");
                }
            }

            Debug.Log($"Wave {currentWaveIndex + 1} spawn complete.");

            if (currentWaveIndex != 4) // Only wait if not final wave
            {
                while (currentEnemyCount.Length > 0)
                {
                    yield return null;
                }
            }

            if (currentWaveIndex < waves.Length - 1)
            {
                if (currentWave.waveDelay > 0)
                {
                    Debug.Log($"Waiting {currentWave.waveDelay} seconds before next wave...");
                    waveDelayTime = currentWave.waveDelay;
                    currentSceneTime = Time.timeSinceLevelLoad + waveDelayTime;
                    yield return new WaitForSeconds(currentWave.waveDelay);
                }
                else
                {
                    yield return null;
                }
            }

            currentWaveIndex++;
        }

        Debug.Log("All waves completed!");
        OnAllWavesComplete?.Invoke();
        GameManager.Instance?.AllWavesComplete();
    }

    public GameObject[] FindGameObjectsWithTag(string[] tags)
    {
        List<GameObject> allObjects = new List<GameObject>();

        foreach (string tag in tags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            allObjects.AddRange(objects);
        }

        return allObjects.ToArray();
    }
}
