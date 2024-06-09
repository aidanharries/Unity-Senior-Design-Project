using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BackButton : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    public Image fadePanel;
    public AudioClip select;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
    }

    public void backButton()
    {
        audioSource.PlayOneShot(select);
        StartCoroutine(FadeAndLoadScene(0));
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
