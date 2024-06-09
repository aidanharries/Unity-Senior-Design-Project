using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class HighScoreDisplay : MonoBehaviour
{
    public List<TextMeshProUGUI> scoreTextList; // List to hold the score text objects
    public List<TextMeshProUGUI> nameTextList; // List to hold the name text objects
    public Button backButton; // Reference to the back button
    public TextButtonColorChange backButtonColorChanger;
    private int currentIndex = 1; // Start on index 1

    private const int MaxHighScores = 10;

    private void Start()
    {
        DisplayHighScores();
    }
    
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") < -0.5f)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                UpdateButtonHighlight();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxis("Horizontal") > 0.5f)
        {
            if (currentIndex < 1)
            {
                currentIndex++;
                UpdateButtonHighlight();
            }
        }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit")))
        {
            switch (currentIndex)
            {
                case 0:
                    backButton.onClick.Invoke(); // Trigger back button click
                    break;
                case 1:
                    // Do Nothing
                    break;
            }
        }
    }

    void UpdateButtonHighlight()
    {
        backButtonColorChanger.ChangeButtonColor(currentIndex == 0);
    }

    private void DisplayHighScores()
    {
        // Get the high score list from HighScoreManager
        var highScoreList = HighScoreManager.Instance != null ? HighScoreManager.Instance.GetHighScores() : new List<HighScoreEntry>();

        for (int i = 0; i < MaxHighScores; i++)
        {
            if (i < highScoreList.Count)
            {
                // Update score and name text objects
                scoreTextList[i].text = highScoreList[i].score.ToString();
                nameTextList[i].text = highScoreList[i].initials;
            }
            else
            {
                // Default text if no score exists for the rank
                scoreTextList[i].text = "..................";
                nameTextList[i].text = ".........";
            }
        }
    }
}

