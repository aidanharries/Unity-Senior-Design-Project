using UnityEngine;

public class HowToPlaySetup : MonoBehaviour
{
    public GameObject[] ships; 

    void Start()
    {
        int selectedShipIndex = ShipSelect.SelectedShipIndex;

        // Set all ships to transparent
        foreach (GameObject ship in ships)
        {
            SpriteRenderer sr = ship.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 0; // Set alpha to 0
                sr.color = color;
            }
        }

        // Make sure the selected ship is within the array bounds
        if (selectedShipIndex >= 0 && selectedShipIndex < ships.Length)
        {
            // Set the selected ship to opaque
            SpriteRenderer selectedSr = ships[selectedShipIndex].GetComponent<SpriteRenderer>();
            if (selectedSr != null)
            {
                Color selectedColor = selectedSr.color;
                selectedColor.a = 1; // Set alpha to 1
                selectedSr.color = selectedColor;
            }
        }
    }
}
