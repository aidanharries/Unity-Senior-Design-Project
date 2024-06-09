using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameStartController : MonoBehaviour
{
    public WaveSpawner waveSpawner;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI enterText;
    public AudioClip select;
    private AudioSource audioSource;
    private bool gameStarted = false;

    void Start()
    {
        waveSpawner.enabled = false;

        // Show instructions
        instructionText.text = "Take some time to figure out the controls.";
        enterText.text = "Press ENTER to start";
        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
    }

    void Update()
    {
        if (!gameStarted && (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit")))
        {
            audioSource.PlayOneShot(select);
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        // Fade out the text elements
        float duration = 1.0f; // 1 second fade
        float currentTime = 0f;

        Color instructionColor = instructionText.color;
        Color enterColor = enterText.color;

        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
            instructionText.color = new Color(instructionColor.r, instructionColor.g, instructionColor.b, alpha);
            enterText.color = new Color(enterColor.r, enterColor.g, enterColor.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }

        // hide the text and start the waves
        instructionText.gameObject.SetActive(false);
        enterText.gameObject.SetActive(false);
        gameStarted = true;

        // Enable the WaveSpawner script to start waves
        waveSpawner.enabled = true;
    }
}
