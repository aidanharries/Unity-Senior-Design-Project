using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 1.0f; // Duration of the fade effect

    private void Start()
    {
        StartCoroutine(FadeInScene());
    }

    private IEnumerator FadeInScene()
    {
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
