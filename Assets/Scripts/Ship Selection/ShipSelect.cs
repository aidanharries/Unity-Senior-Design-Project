using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShipSelect : MonoBehaviour
{
    public static int SelectedShipIndex { get; private set; } = 1;
    public float fadeDuration = 1.0f;
    public Image fadePanel;
    public TextMeshProUGUI enterText;
    public Color pressedColor = Color.red; 
    public TextButtonColorChange backButtonColorChanger;
    public Button backButton;
    public AudioClip select;

    private bool isBackButtonSelected = false;
    private float inputCooldown = 0.2f; // Cooldown time in seconds
    private float lastInputTime;
    private AudioSource audioSource;

    private Vector3[] shipPositions = 
    {
        new Vector3(-324, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(324, 0, 0)
    };
    public int currentIndex = 1;

    public void SetSelectorPosition(int index)
    {
        if (index >= 0 && index < shipPositions.Length)
        {
            currentIndex = index;
        }
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
    }

    void Update()
    {
        HandleInput();
        MoveSelector();
    }

    void HandleInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (Time.time - lastInputTime > inputCooldown)
        {
            if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || horizontalInput > 0.5f))
            {
                if (isBackButtonSelected)
                {
                    isBackButtonSelected = false;
                    backButtonColorChanger.ChangeButtonColor(false);
                    SetAlpha(gameObject, 1f);
                }
                else if (currentIndex < shipPositions.Length - 1)
                {
                    currentIndex++;
                }
                lastInputTime = Time.time; 
            }
            else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || horizontalInput < -0.5f))
            {
                if (currentIndex > 0)
                {
                    currentIndex--;
                    lastInputTime = Time.time; 
                }
                else if (currentIndex == 0)
                {
                    isBackButtonSelected = true;
                    backButtonColorChanger.ChangeButtonColor(true);
                    SetAlpha(gameObject, 0f);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit"))
        {
            audioSource.PlayOneShot(select);

            if (isBackButtonSelected)
            {
                backButton.onClick.Invoke();
            }
            else
            {
                enterText.color = pressedColor;
                SelectedShipIndex = currentIndex;
                StartCoroutine(FadeAndLoadScene(4)); 
            }
        }
    }

    void MoveSelector()
    {
        if (!isBackButtonSelected)
        {
            transform.localPosition = shipPositions[currentIndex];
            SetAlpha(gameObject, 1f);
        }
        else
        {
            SetAlpha(gameObject, 0f);
        }
    }

    private void SetAlpha(GameObject obj, float alpha)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
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