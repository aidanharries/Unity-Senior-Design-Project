using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalResourceMultiplier : MonoBehaviour
{
    public static GlobalResourceMultiplier Instance;

    public int CurrentMultiplier { get; private set; } = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncreaseMultiplier()
    {
        CurrentMultiplier++;
        Debug.Log("Resource Multiplier increased to: " + CurrentMultiplier);
    }

    public void ResetMultiplier()
    {
        CurrentMultiplier = 1; // Reset to initial value
    }
}

