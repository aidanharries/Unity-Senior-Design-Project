using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    public TextMeshProUGUI[] resourceTexts; // Array of UI Text elements that will display the resource count
    private int resourceCount;
    private List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResetResources();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void UpdateAllUpgradeButtonStatuses()
    {
        foreach (var button in upgradeButtons)
        {
            button.UpdateUpgradeButtonStatus();
        }
    }

    public void RegisterUpgradeButton(UpgradeButton button)
    {
        if (!upgradeButtons.Contains(button))
        {
            upgradeButtons.Add(button);
            button.UpdateUpgradeButtonStatus();
        }
    }

    public void UnregisterUpgradeButton(UpgradeButton button)
    {
        upgradeButtons.Remove(button);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateResourceUI();
    }

    public void AddCount(int count)
    {
        resourceCount += count;
        UpdateResourceUI();
    }

    private void UpdateResourceUI()
    {
        foreach (var text in resourceTexts)
        {
            text.text = "" + resourceCount;
        }
    }

    public bool HasEnoughResources(int requiredAmount)
    {
        return resourceCount >= requiredAmount;
    }

    public void ResetResources()
    {
        resourceCount = 0;
        UpdateResourceUI();
    }

    // New method to set the resource text references from external scripts.
    public void SetResourceText(TextMeshProUGUI[] newTexts)
    {
        resourceTexts = newTexts;
        UpdateResourceUI(); // Update all UI elements at once
    }
}
