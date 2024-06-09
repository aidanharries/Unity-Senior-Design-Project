using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private TextMeshProUGUI scoreText; // Reference to the UI Text element
    private int score;
    private int displayedScore;

    private void Awake()
    {
        // Singleton pattern to ensure only one ScoreManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the ScoreManager across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        StopAllCoroutines(); // Stop the current coroutine if it's running
        StartCoroutine(UpdateScoreRoutine());
    }

    private IEnumerator UpdateScoreRoutine()
    {
        while (displayedScore < score)
        {
            displayedScore++;
            UpdateScoreText();
            yield return new WaitForSeconds(0.01f); // Time delay between increments
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "" + displayedScore;
        }
    }

    public int GetCurrentScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0;
        displayedScore = 0;
        UpdateScoreText();
    }

    public void SetScoreText(TextMeshProUGUI newText)
    {
        scoreText = newText;
        UpdateScoreText(); // Update the score display immediately
    }
}
