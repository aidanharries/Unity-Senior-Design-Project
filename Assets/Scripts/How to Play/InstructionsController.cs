using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class InstructionsController : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    public Image fadePanel;
    public GameObject[] instructionStages;
    public GameObject[] playerShips;
    public GameObject pressEnter;
    public GameObject[] walls;
    public GameObject planet;
    public GameObject resource;
    public Button backButton; // Reference to the back button
    public Button skipButton; // Reference to the skip button
    public TextButtonColorChange backButtonColorChanger;
    public TextButtonColorChange skipButtonColorChanger;
    public AudioClip select;

    private AudioSource audioSource;
    private int currentIndex = 1; // Start on index 1
    private int currentStage = 0;
    private HowToPlaySetup howToPlay;
    private bool isAnimationPlaying = false; 
    private float inputCooldown = 0.2f; // Cooldown time in seconds
    private float lastInputTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
        ShowCurrentStage();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (Time.time - lastInputTime > inputCooldown)
        {
            if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || horizontalInput > 0.5f))
            {
                if (currentIndex < 2)
                {
                    currentIndex++;
                    UpdateButtonHighlight();
                }
                lastInputTime = Time.time; 
            }
            else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || horizontalInput < -0.5f))
            {
                if (currentIndex > 0)
                {
                    currentIndex--;
                    UpdateButtonHighlight();
                }
                lastInputTime = Time.time; 
            }
        }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit")) && !isAnimationPlaying)
        {
            audioSource.PlayOneShot(select);
            switch (currentIndex)
            {
                case 0:
                    backButton.onClick.Invoke(); // Trigger back button click
                    break;
                case 1:
                    StartCoroutine(AdvanceInstruction()); // Start the AdvanceInstruction coroutine
                    break;
                case 2:
                    skipButton.onClick.Invoke(); // Trigger skip button click
                    break;
            }
        }
    }

    void UpdateButtonHighlight()
    {
        backButtonColorChanger.ChangeButtonColor(currentIndex == 0);
        skipButtonColorChanger.ChangeButtonColor(currentIndex == 2);
    }

    IEnumerator AdvanceInstruction()
    {
        isAnimationPlaying = true;

        // Hide animations for all children
        PlayAnimationForAllChildren(instructionStages[currentStage], "Hide");

        // Hide the PressEnter prompt
        if (pressEnter.activeSelf)
        {
            Animation enterAnimation = pressEnter.GetComponent<Animation>();
            if (enterAnimation && enterAnimation.GetClip("Hide") != null)
            {
                enterAnimation.Play("Hide");
            }
        }

        if (currentStage == 1)
        {
            // Hide both walls
            foreach (var wall in walls)
            {
                Animation wallAnimation = wall.GetComponent<Animation>();
                if (wallAnimation && wallAnimation.GetClip("HideSprite") != null)
                {
                    wallAnimation.Play("HideSprite");
                }
            }

            // Hide chosen ship
            foreach (GameObject ship in playerShips)
            {
                SpriteRenderer renderer = ship.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    float alpha = renderer.color.a;
                    if (Mathf.Approximately(alpha, 1f)) // Checks if the alpha is approximately 1
                    {
                        Animation shipAnimation = ship.GetComponent<Animation>();
                        shipAnimation.Play("HideSprite");
                    }
                }
            }
        }
        else if (currentStage == 3) // Check if we are in instruction set 4
        {
            // Hide the resource
            Animation resourceAnimation = resource.GetComponent<Animation>();
            if (resourceAnimation && resourceAnimation.GetClip("HideSprite") != null)
            {
                resourceAnimation.Play("HideSprite");
            }

            // Hide the walls
            foreach (var wall in walls)
            {
                Animation wallAnimation = wall.GetComponent<Animation>();
                if (wallAnimation && wallAnimation.GetClip("HideSprite") != null)
                {
                    wallAnimation.Play("HideSprite");
                }
            }
        }
        
        Animation animation = instructionStages[currentStage].GetComponent<Animation>();
        if (animation)
        {
            animation.Play("Hide");

            // Check if we're moving from stage 0 to stage 1
            if (currentStage == 0)
            {
                // Play the "MoveShip" animation for all player ships
                foreach (var playerShip in playerShips)
                {
                    Animation shipAnimation = playerShip.GetComponent<Animation>();
                    if (shipAnimation && shipAnimation.GetClip("MoveShip") != null)
                    {
                        shipAnimation.Play("MoveShip");
                    }
                }

                // Play the "WallShow" animation for both walls
                foreach (var wall in walls)
                {
                    Animation wallAnimation = wall.GetComponent<Animation>();
                    if (wallAnimation && wallAnimation.GetClip("WallShow1") != null)
                    {
                        wallAnimation.Play("WallShow1");
                    }
                    else if (wallAnimation && wallAnimation.GetClip("WallShow2") != null)
                    {
                        wallAnimation.Play("WallShow2");
                    }
                }
            }

            // Check if we're moving from stage 1 to stage 2
            if (currentStage == 1)
            {
                // Play "ShowSprite" animation for planet
                Animation planetAnimation = planet.GetComponent<Animation>();
                if (planetAnimation && planetAnimation.GetClip("ShowPlanet") != null)
                {
                    planetAnimation.Play("ShowPlanet");
                }
            }

            // Check if we're moving from stage 2 to stage 3
            if (currentStage == 2)
            {
                Animation planetAnimation = planet.GetComponent<Animation>();
                if (planetAnimation && planetAnimation.GetClip("MovePlanet") != null)
                {
                    planetAnimation.Play("MovePlanet");
                }

                // Play the "WallShow" animation for top wall
                foreach (var wall in walls)
                {
                    Animation wallAnimation = wall.GetComponent<Animation>();
                    if (wallAnimation && wallAnimation.GetClip("WallShow2") != null)
                    {
                        wallAnimation.Play("WallShow2");
                    }
                }

                // Play the "ResourceShow" animation
                Animation resourceAnimation = resource.GetComponent<Animation>();
                if (resourceAnimation && resourceAnimation.GetClip("ResourceShow") != null)
                {
                    resourceAnimation.Play("ResourceShow");
                }
            }

            // Wait for the "Hide" animation to finish
            yield return new WaitWhile(() => animation.isPlaying);
        }

        isAnimationPlaying = false;
        currentStage++;
        ShowCurrentStage();
    }

    IEnumerator PlayHideAnimationAndFinish()
    {
        isAnimationPlaying = true;

        // Hide animations for all children
        PlayAnimationForAllChildren(instructionStages[currentStage], "Hide");
        PlayAnimationForAllChildren(instructionStages[currentStage], "HideSprite");

        // Hide the PressEnter prompt
        if (pressEnter.activeSelf)
        {
            Animation enterAnimation = pressEnter.GetComponent<Animation>();
            if (enterAnimation && enterAnimation.GetClip("Hide") != null)
            {
                enterAnimation.Play("Hide");
            }
        }

        Animation animation = instructionStages[currentStage].GetComponent<Animation>();
        if (animation)
        {
            animation.Play("Hide");
            yield return new WaitWhile(() => animation.isPlaying);
        }

        isAnimationPlaying = false;
        FinishInstructions();
    }

    void ShowCurrentStage()
    {
        foreach (var stage in instructionStages)
        {
            stage.SetActive(false);
        }

        // Check if currentStage index is within the bounds of the array
        if (currentStage >= 0 && currentStage < instructionStages.Length)
        {
            GameObject currentStageObject = instructionStages[currentStage];
            currentStageObject.SetActive(true);

            // Show animations for all children
            PlayAnimationForAllChildren(currentStageObject, "Show");

            Animation animation = currentStageObject.GetComponent<Animation>();
            if (animation && animation.GetClip("Show") != null)
            {
                animation.Play("Show");
            }

            // Play the PressEnter "Show" animation if it exists
            if (pressEnter != null)
            {
                Animation enterAnimation = pressEnter.GetComponent<Animation>();
                if (enterAnimation && enterAnimation.GetClip("Show") != null)
                {
                    enterAnimation.Play("Show");
                    pressEnter.SetActive(true);
                }
            }
        }
        else
        {
            // Handle cases where currentStage is out of bounds, such as finishing the instructions
            // For example:
            FinishInstructions();
        }
    }

    void PlayAnimationForAllChildren(GameObject parent, string animationName)
    {
        foreach (Transform child in parent.transform)
        {
            Animation childAnimation = child.GetComponent<Animation>();
            if (childAnimation && childAnimation.GetClip(animationName) != null)
            {
                childAnimation.Play(animationName);
            }
        }
    }

    void FinishInstructions()
    {
        StartCoroutine(FadeAndLoadScene(5)); // Load Gameplay Scene
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