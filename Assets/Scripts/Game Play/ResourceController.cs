using UnityEngine;
using TMPro;

public class ResourceController : MonoBehaviour
{
    public TextMeshProUGUI[] resourceTexts;

    private void Start()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.SetResourceText(resourceTexts);
        }
    }
}
