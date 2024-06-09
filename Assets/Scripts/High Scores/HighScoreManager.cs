using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;
    public HighScores highScores = new HighScores();

    private string filePath;
    private const int MaxHighScores = 10;

    private void Awake()
    {
        Instance = this;
        filePath = Application.persistentDataPath + "/highscores.json";
        Debug.Log("High scores file path: " + filePath);
        LoadHighScores();
    }

    public void AddHighScore(int score, string initials = "ABC")
    {
        if (IsHighScore(score))
        {
            Debug.Log($"Adding high score: {score}, {initials}");
            highScores.highScoreList.Add(new HighScoreEntry { score = score, initials = initials });
            highScores.highScoreList = highScores.highScoreList.OrderByDescending(h => h.score).ToList();

            if (highScores.highScoreList.Count > MaxHighScores)
            {
                highScores.highScoreList.RemoveRange(MaxHighScores, highScores.highScoreList.Count - MaxHighScores);
            }

            SaveHighScores();
        }
        else
        {
            Debug.Log($"Score {score} did not qualify as a high score.");
        }
    }

    private void LoadHighScores()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            highScores = JsonUtility.FromJson<HighScores>(json);
        }
    }

    private void SaveHighScores()
    {
        string json = JsonUtility.ToJson(highScores);
        File.WriteAllText(filePath, json);
    }

    public bool IsHighScore(int score)
    {
        // Exclude scores of 0 or lower
        if (score <= 0)
        {
            return false;
        }

        // Check if the list has less than 10 scores
        if (highScores.highScoreList.Count < MaxHighScores)
        {
            return true;
        }

        // Check if the score is higher than the lowest score in the top 10
        return score > highScores.highScoreList[MaxHighScores - 1].score;
    }

    public List<HighScoreEntry> GetHighScores()
    {
        return highScores.highScoreList;
    }
}
