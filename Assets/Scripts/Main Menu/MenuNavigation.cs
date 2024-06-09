using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuNavigation : MonoBehaviour
{
    public Button[] buttons; 
    public TextMeshProUGUI[] buttonTexts; 
    public Color normalColor = Color.white;
    public Color highlightedColor = Color.yellow;
    public Color pressedColor = Color.red; 
    public MainMenu mainMenu; 
    public AudioClip select;

    private int currentSelectedIndex = -1; 
    private Vector3 lastMousePosition; 
    private float lastJoystickVerticalInput; 
    private AudioSource audioSource;

    private void Start()
    {
        UpdateButtonColors(); 
        lastMousePosition = Input.mousePosition;
        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
    }
    
    void Update()
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
            audioSource.PlayOneShot(select);
            buttonTexts[currentSelectedIndex].color = pressedColor;
            switch (currentSelectedIndex)
            {
                case 0:
                    MainMenu.Instance.LoadShipSelectionScene();
                    break;
                case 1:
                    MainMenu.Instance.LoadHighScoresScene();
                    break;
                case 2:
                    MainMenu.Instance.LoadSettingsScene();
                    break;
                case 3:
                    MainMenu.Instance.Quit();
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
}
