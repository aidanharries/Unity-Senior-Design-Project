using UnityEngine;
using TMPro;

public class HealthTextController : MonoBehaviour
{
    public TextMeshProUGUI planetText;
    public TextMeshProUGUI playerText;
    private PlayerHealth playerHealth;
    private PlanetHealth planetHealth;

    void Update()
    {
        // Find the PlayerHealth component if not already found
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            // Update player health text
            playerText.text = $"Player Health: ({playerHealth.health} / {playerHealth.maxHealth})";
        }

        // Find the PlanetHealth component if not already found
        if (planetHealth == null)
        {
            planetHealth = FindObjectOfType<PlanetHealth>();
        }

        if (planetHealth != null)
        {
            // Update planet health text
            planetText.text = $"Planet Health: ({planetHealth.health} / {planetHealth.maxHealth})";
        }
    }
}
