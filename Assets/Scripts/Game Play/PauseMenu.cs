using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // The pause menu panel
    public WaveSpawner waveSpawner; // Reference to the WaveSpawner to check if the upgrade menu is active
    public Button[] buttons; 
    public TextMeshProUGUI[] buttonTexts; 
    public Color normalColor = Color.white;
    public Color highlightedColor = Color.yellow;
    public Color pressedColor = Color.red; 

    private int currentSelectedIndex = -1; 
    private Vector3 lastMousePosition; 
    private float lastJoystickVerticalInput; 

    void Start()
    {
        pauseMenuUI.SetActive(false); // Start with the pause menu not visible
        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
        if (pauseMenuUI.activeSelf) // Only run if the pause menu is active
        {
            HandleInput();
        }
    }

    public void OnPause(InputValue value)
    {
        Debug.Log($"Pause pressed: {value.isPressed}");
        if (value.isPressed && !waveSpawner.IsUpgradeMenuActive())
        {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        pauseMenuUI.SetActive(!pauseMenuUI.activeSelf);
        Time.timeScale = pauseMenuUI.activeSelf ? 0 : 1;
    }

    private void HandleInput()
    {
        if (lastMousePosition != Input.mousePosition)
        {
            if (currentSelectedIndex != -1)
            {
                currentSelectedIndex = -1;
                UpdateButtonColors();
            }
            lastMousePosition = Input.mousePosition;
        }

        float joystickVerticalInput = Input.GetAxisRaw("Vertical");

        // Check for vertical input with debounce
        if (Mathf.Abs(joystickVerticalInput) > 0.5f && Mathf.Abs(lastJoystickVerticalInput) <= 0.5f)
        {
            if (joystickVerticalInput < 0)
            {
                currentSelectedIndex = (currentSelectedIndex + 1) % buttons.Length;
            }
            else if (joystickVerticalInput > 0)
            {
                currentSelectedIndex = (currentSelectedIndex - 1 + buttons.Length) % buttons.Length;
            }
            UpdateButtonColors();
        }

        lastJoystickVerticalInput = joystickVerticalInput;

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit")) && currentSelectedIndex != -1) 
        {
            buttonTexts[currentSelectedIndex].color = pressedColor;
            switch (currentSelectedIndex)
            {
                case 0:
                    ResumeGame();
                    break;
                case 1:
                    LoadMainMenu();
                    break;
                case 2:
                    QuitGame();
                    break;
            }
        }
    }

    private void UpdateButtonColors()
    {
        for (int i = 0; i < buttonTexts.Length; i++)
        {
            buttonTexts[i].color = normalColor;
        }
        if (currentSelectedIndex != -1)
        {
            buttonTexts[currentSelectedIndex].color = highlightedColor;
        }
    }

    public void ResumeGame()
    {
        // Resume game logic
        TogglePauseMenu();
    }

    void Pause()
    {
        // Pause game logic
        TogglePauseMenu();
    }

    public void LoadMainMenu()
    {
        ResetGameStats();

        // Logic to load the main menu
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
