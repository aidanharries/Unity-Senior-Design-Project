using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour
{
    public enum UpgradeType
    {
        ThrustSpeed,
        ShootTimer,
        EnemyDamage,
        ShipHealth,
        DefenseTurret,
        ResourceMultiplier,
        Moon,
        PlanetHealth,
        ContinueGame
    }

    public TextMeshProUGUI buttonText;
    public Button upgradeButton;
    public Image childImage;
    public int upgradeCost;
    public UpgradeType upgradeType; // Public field to set in Unity Inspector
    public Color originalNormalColor; // Add this field
    public AudioClip select;
    private AudioSource audioSource;
    private int currentLevel = 0;
    private int maxLevel = 4;
    private Color originalTextColor;
    private Color disabledTextColor = Color.grey;
    private bool IsContinueGameAction = false;
    private Action upgradeAction; // Delegate for upgrade action

    private void Awake()
    {
        originalTextColor = buttonText.color;
        UpdateButtonText(); // Update the text immediately
        RegisterWithResourceManager();
        originalNormalColor = upgradeButton.colors.normalColor;
        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
    }

    private void Update()
    {
        UpdateUpgradeButtonStatus();
    }

    private void RegisterWithResourceManager()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.RegisterUpgradeButton(this);
        }
    }

    private void OnEnable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.RegisterUpgradeButton(this);
            UpdateUpgradeButtonStatus();
        }
    }

    private void OnDisable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.UnregisterUpgradeButton(this);
        }
    }

    public void OnUpgradeButtonClicked()
    {        
        Player player = FindObjectOfType<Player>();
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        Planet planet = FindObjectOfType<Planet>();
        PlanetHealth planetHealth = FindObjectOfType<PlanetHealth>();
        WaveSpawner waveSpawner = FindObjectOfType<WaveSpawner>();

        // Assign the appropriate action based on the upgradeType
        switch (upgradeType)
        {
            case UpgradeType.ThrustSpeed:
                audioSource.PlayOneShot(select);
                upgradeAction = () => player.IncreaseThrustSpeed(1.0f);
                break;
            case UpgradeType.ShootTimer:
                audioSource.PlayOneShot(select);
                upgradeAction = () => player.DecreaseShootTimer(0.25f);
                break;
            case UpgradeType.EnemyDamage:
                audioSource.PlayOneShot(select);
                upgradeAction = () => player.DecreaseEnemyDamage(1.0f);
                break;
            case UpgradeType.ShipHealth:
                audioSource.PlayOneShot(select);
                upgradeAction = () => playerHealth.IncreaseMaxHealth(25f);
                break;
            case UpgradeType.DefenseTurret:
                audioSource.PlayOneShot(select);
                upgradeAction = () => planet.AddTurret();
                break;
            case UpgradeType.ResourceMultiplier:
                audioSource.PlayOneShot(select);
                upgradeAction = () => GlobalResourceMultiplier.Instance.IncreaseMultiplier();
                break;
            case UpgradeType.Moon:
                audioSource.PlayOneShot(select);
                upgradeAction = () => planet.IncreaseMoonSpeed();
                break;
            case UpgradeType.PlanetHealth:
                audioSource.PlayOneShot(select);
                upgradeAction = () => planetHealth.IncreaseMaxHealth(25f);
                break;
            case UpgradeType.ContinueGame:
                IsContinueGameAction = true;
                upgradeAction = () => waveSpawner.ContinueGame();
                break;
            default:
                IsContinueGameAction = false;
                break;
        }

        if (currentLevel < maxLevel && ResourceManager.Instance.HasEnoughResources(upgradeCost))
        {
            if (!IsContinueGameAction)
            {
                ResourceManager.Instance.AddCount(-upgradeCost);
                currentLevel++;
                upgradeCost += 10;

                UpdateButtonText();
            }

            upgradeAction?.Invoke();

            // Deselect the button
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void UpdateUpgradeButtonStatus()
    {
        if (ResourceManager.Instance == null || upgradeButton == null || buttonText == null || childImage == null)
        {
            return;
        }

        bool hasEnoughResources = ResourceManager.Instance.HasEnoughResources(upgradeCost);
        upgradeButton.interactable = hasEnoughResources && currentLevel < maxLevel;
        buttonText.color = hasEnoughResources ? originalTextColor : disabledTextColor;
        childImage.enabled = currentLevel < maxLevel;
    }

    public void UpdateButtonText()
    {
        if (buttonText == null) return;

        buttonText.text = currentLevel >= maxLevel 
            ? "MAXED OUT" 
            : $"COST: {upgradeCost}\nLEVEL: {currentLevel}/{maxLevel}";
        UpdateUpgradeButtonStatus();
    }
}
