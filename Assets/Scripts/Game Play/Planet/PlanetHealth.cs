using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetHealth : MonoBehaviour
{
    public Transform fillTransform;
    private SpriteRenderer fillRenderer;
    public float health = 500f;
    public float maxHealth = 500f;

    // Define public variables for colors
    public Color highHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    private void Start()
    {
        fillRenderer = fillTransform.GetComponent<SpriteRenderer>();
        SetHealth(health, maxHealth); // Set initial health
    }

    // Method to increase max health
    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        health = maxHealth; // refill health to max
        SetHealth(health, maxHealth);
    }

    public void ChangeHealth(float amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth); // Ensure health stays within bounds
        SetHealth(health, maxHealth);
    }

    public void SetHealth(float health, float maxHealth)
    {
        float fillAmount = health / maxHealth;
        fillTransform.localScale = new Vector3(fillAmount, 1f, 1f);

        // Change color based on health percentage
        if (fillAmount > 0.7f) // 100-70%
        {
            fillRenderer.color = highHealthColor;
        }
        else if (fillAmount > 0.3f) // 69-30%
        {
            fillRenderer.color = mediumHealthColor;
        }
        else // 29-1%
        {
            fillRenderer.color = lowHealthColor;
        }

        // // Check if health reaches zero
        // if (health <= 0)
        // {
        //     // Load the GameOver scene
        //     SceneManager.LoadScene("GameOver");
        // }

        // Debug the health percentage
        float healthPercentage = fillAmount * 100f;
        Debug.Log("Health: " + healthPercentage + "%");
    }
}
