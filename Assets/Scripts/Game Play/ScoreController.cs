using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    public TextMeshProUGUI gameplayScoreText;

    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetScoreText(gameplayScoreText);
        }
    }
}
