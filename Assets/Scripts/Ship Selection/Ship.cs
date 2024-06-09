using UnityEngine;

public class Ship : MonoBehaviour
{
    public int shipIndex = 0;
    private ShipSelect selector;

    void Start()
    {
        selector = FindObjectOfType<ShipSelect>();
    }

    void OnMouseDown()
    {
        if (selector)
        {
            selector.SetSelectorPosition(shipIndex);
        }
    }
}
