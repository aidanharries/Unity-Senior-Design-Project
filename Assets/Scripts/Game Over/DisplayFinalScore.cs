using UnityEngine;
using TMPro;

public class DisplayFinalScore : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalWaveNumber;
    public TextMeshProUGUI subtitleText;

    public TMP_InputField initialsInputField;
    public TextMeshProUGUI enterInitialsText;
    public TextMeshProUGUI pressEnterText;

    public GameObject playAgainButton;
    public GameObject mainMenuButton;

    public GameOverNavigation gameOverNavigation;

    void Start()
    {
        int finalScore = 0;
        if (ScoreManager.Instance != null)
        {
            finalScore = ScoreManager.Instance.GetCurrentScore();
            finalScoreText.text = "" + finalScore;
        }

        if (WaveManager.Instance != null)
        {
            int waveNum = WaveManager.Instance.GetCurrentWave() - 1;
            finalWaveNumber.text = "" + waveNum;
        }

        UpdateSubtitle(finalScore);
    }

    private void UpdateSubtitle(int score)
    {
        bool isHighScore = HighScoreManager.Instance != null && HighScoreManager.Instance.IsHighScore(score);

        if (isHighScore)
        {
            subtitleText.text = "NEW HIGH SCORE";
            subtitleText.color = Color.yellow;
            // Hide the buttons, show the input field
            playAgainButton.SetActive(false);
            mainMenuButton.SetActive(false);
            enterInitialsText.gameObject.SetActive(true);
            pressEnterText.gameObject.SetActive(true);
            initialsInputField.gameObject.SetActive(true);

            if (gameOverNavigation != null)
            {
                gameOverNavigation.enabled = false; // Disable navigation
            } 
        }
        else
        {
            subtitleText.text = "Great Effort!";
            subtitleText.color = Color.white;
            // Show the buttons, hide the input field
            playAgainButton.SetActive(true);
            mainMenuButton.SetActive(true);
            enterInitialsText.gameObject.SetActive(false);
            pressEnterText.gameObject.SetActive(false);
            initialsInputField.gameObject.SetActive(false);

            if (gameOverNavigation != null)
            {
                gameOverNavigation.enabled = true; // Enable navigation
            }
        }
    }
}
