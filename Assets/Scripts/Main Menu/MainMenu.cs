using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    public Image fadePanel;
    public float fadeDuration = 1.0f;
    public AudioClip select;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void LoadShipSelectionScene()
    {
        audioSource.PlayOneShot(select);
        StartCoroutine(FadeAndLoadScene(1));
    }

    public void LoadHighScoresScene()
    {
        audioSource.PlayOneShot(select);
        StartCoroutine(FadeAndLoadScene(2));
    }

    public void LoadSettingsScene()
    {
        audioSource.PlayOneShot(select);
        StartCoroutine(FadeAndLoadScene(3));
    }

    public void Quit()
    {
        audioSource.PlayOneShot(select);
        Application.Quit();
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