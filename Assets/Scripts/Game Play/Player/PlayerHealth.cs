using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Transform fillTransform;
    private SpriteRenderer fillRenderer;
    public float health = 100f;
    public float maxHealth = 100f;

    // Define public variables for colors
    public Color highHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    private static float originalMaxHealth = 100f;

    private void Awake()
    {
        ResetSpeedToOriginal();
        fillRenderer = fillTransform.GetComponent<SpriteRenderer>();
        SetHealth(health, maxHealth); // Set initial health
    }

    public void ResetSpeedToOriginal()
    {
        maxHealth = originalMaxHealth;
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

        // Debug the health percentage
        float healthPercentage = fillAmount * 100f;
        Debug.Log("Health: " + healthPercentage + "%");
    }
}
