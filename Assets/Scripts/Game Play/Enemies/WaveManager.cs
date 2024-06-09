using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    private int currentWave;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the WaveManager across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void NextWave()
    {
        currentWave++;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public void ResetWave()
    {
        currentWave = 0;
    }
}
