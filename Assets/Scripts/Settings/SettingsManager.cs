using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//using System.Collections.Generic;
//using TMPro;

public class SettingsManager : MonoBehaviour
{
    public Button backButton; // Reference to the back button
    public TextButtonColorChange backButtonColorChanger;
    private int currentIndex = 1; // Start on index 1

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
}
