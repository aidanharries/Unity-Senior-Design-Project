using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class EnemySpawn
{
    public GameObject enemyPrefab;
    public int count;
}

[System.Serializable]
public class Wave
{
    public EnemySpawn[] enemySpawns;
    public float timeBetweenSpawns;
}

public class WaveSpawner : MonoBehaviour
{
    public Wave[] waves;
    public Transform[] spawnPoints;
    public TextMeshProUGUI waveNumberText;
    public GameObject upgradeMenu;
    public float timeBetweenWaves = 5f;
    public Button continueGameButton;
    public AudioClip select;

    private AudioSource audioSource;
    private Wave currentWave;
    private int currentWaveNumber;
    private int waveCount;
    private float nextWaveTime;
    private bool isSpawningWave = false;
    private bool isUpgrading = false;
    private int activeEnemyCount = 0;
    private CanvasGroup upgradeMenuCanvasGroup;

    private void Start()
    {
        nextWaveTime = Time.time + timeBetweenWaves;
        waveCount = 0;
        upgradeMenu.SetActive(false);
        upgradeMenuCanvasGroup = upgradeMenu.GetComponent<CanvasGroup>(); // Initialize canvas group
        upgradeMenuCanvasGroup.alpha = 0f; // Start with the menu fully transparent
        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
    }

    private void Update()
    {
        if (Time.time >= nextWaveTime && !isSpawningWave && activeEnemyCount == 0 && !isUpgrading)
        {
            StartCoroutine(PrepareWave());
            if (waveCount > 0)
            {
                ScoreManager.Instance.AddScore(500); // Add 500 points for surviving a wave
            }
        }
    }

    IEnumerator PrepareWave()
    {
        isSpawningWave = true;
        
        // Display "Wave {wave #}" text
        waveNumberText.text = $"WAVE {waveCount + 1}";
        waveNumberText.gameObject.SetActive(true); // Make sure the text is active
        
        // Fade in the wave number text
        yield return StartCoroutine(FadeTextToFullAlpha(1, waveNumberText));
        
        // Wait for 2 seconds with the full wave text
        yield return new WaitForSeconds(2f);
        
        // Fade out the wave number text
        yield return StartCoroutine(FadeTextToZeroAlpha(1, waveNumberText));
        
        // Wait for 2 seconds before starting the next wave
        yield return new WaitForSeconds(2f);

        // Start spawning the wave
        StartCoroutine(SpawnWave());
    }

    IEnumerator FadeTextToFullAlpha(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    IEnumerator FadeTextToZeroAlpha(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        i.gameObject.SetActive(false); // Deactivate the text game object after fade out
    }

    IEnumerator SpawnWave()
    {
        WaveManager.Instance.NextWave();
        waveCount++;
        currentWave = waves[currentWaveNumber];

        List<GameObject> enemiesToSpawn = new List<GameObject>();
        foreach (var enemySpawn in currentWave.enemySpawns)
        {
            for (int i = 0; i < enemySpawn.count; i++)
            {
                enemiesToSpawn.Add(enemySpawn.enemyPrefab);
            }
        }

        while (enemiesToSpawn.Count > 0)
        {
            int index = Random.Range(0, enemiesToSpawn.Count);
            SpawnEnemy(enemiesToSpawn[index]);
            enemiesToSpawn.RemoveAt(index);
            activeEnemyCount++;
            yield return new WaitForSeconds(currentWave.timeBetweenSpawns);
        }

        currentWaveNumber++;
        if (currentWaveNumber >= waves.Length)
        {
            currentWaveNumber = 0;
        }

        // Wait for all enemies to be destroyed
        yield return new WaitUntil(() => activeEnemyCount == 0);

        // Wait for 3 seconds after the last enemy is destroyed
        yield return new WaitForSeconds(3f);

        // Show upgrade menu
        ShowUpgradeMenu();
    }

    void ShowUpgradeMenu()
    {
        isUpgrading = true;
        upgradeMenu.SetActive(true);
        StartCoroutine(FadeInUpgradeMenu());
        Time.timeScale = 0; // Pause the game
    }

    IEnumerator FadeInUpgradeMenu()
    {
        float duration = 1f; // Duration of the fade
        float currentTime = 0f;

        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / duration);
            upgradeMenuCanvasGroup.alpha = alpha;
            currentTime += Time.unscaledDeltaTime;
            yield return null;
        }

        upgradeMenuCanvasGroup.alpha = 1f; // Ensure the menu is fully visible
    }

    // Method to check if the upgrade menu is active
    public bool IsUpgradeMenuActive()
    {
        return isUpgrading;
    }

    public void ContinueGame()
    {
        audioSource.PlayOneShot(select);
        StartCoroutine(FadeOutUpgradeMenu());
    }

    IEnumerator FadeOutUpgradeMenu()
    {
        float duration = 1f; // Duration of the fade
        float currentTime = 0f;

        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
            upgradeMenuCanvasGroup.alpha = alpha;
            currentTime += Time.unscaledDeltaTime;
            yield return null;
        }

        upgradeMenuCanvasGroup.alpha = 0f;
        upgradeMenu.SetActive(false);
        Time.timeScale = 1; // Resume the game
        isUpgrading = false;
        nextWaveTime = Time.time + timeBetweenWaves;
        isSpawningWave = false;
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, randomPoint.position, Quaternion.identity);
    }

    public void EnemyDestroyed()
    {
        activeEnemyCount--;
        if (activeEnemyCount <= 0 && !isSpawningWave)
        {
            nextWaveTime = Time.time + timeBetweenWaves;
        }
    }
}