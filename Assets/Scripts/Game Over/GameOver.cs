using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance;
    public Image fadePanel;
    public float fadeDuration = 1.0f;
    public HighScoreManager highScoreManager;
    public TMP_InputField initialsInputField;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Subscribe to the OnEndEdit event of the input field
        initialsInputField.onEndEdit.AddListener(HandleInitialsSubmit);
    }

    public void HandleInitialsSubmit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SubmitHighScore(input); // Use the input directly
        }
    }

    public void SubmitHighScore(string initials)
    {
        int currentScore = ScoreManager.Instance != null ? ScoreManager.Instance.GetCurrentScore() : 0;
        
        // Check if the initials are empty and use a default value
        initials = string.IsNullOrWhiteSpace(initials) ? "ABC" : initials.ToUpper();

        // Add the high score
        if (highScoreManager != null)
        {
            highScoreManager.AddHighScore(currentScore, initials);
        }

        // Optionally reset and hide the input field
        initialsInputField.text = "";

        // Load the highscore screen
        LoadHighScoreScene();
    }

    public void LoadHighScoreScene()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.isHighScoreFromGameOver = true;
        }
        ResetGameStats();
        StartCoroutine(FadeAndLoadScene(2));
    }

    public void LoadGamePlayScene()
    {
        ResetGameStats();
        StartCoroutine(FadeAndLoadScene(5));
    }

    public void LoadMainMenuScene()
    {
        ResetGameStats();
        StartCoroutine(FadeAndLoadScene(0));
    }

    private void ResetGameStats()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
            Debug.Log("Reset Score");
        }
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.ResetWave();
            Debug.Log("Reset Wave");
        }
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.ResetResources();
            Debug.Log("Reset Resources");
        }
        if (GlobalResourceMultiplier.Instance != null)
        {
            GlobalResourceMultiplier.Instance.ResetMultiplier();
            Debug.Log("Reset Resource Multiplier");
        }
    }

    private IEnumerator FadeAndLoadScene(int scene)
    {
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        SceneManager.LoadScene(scene);
    }
}
