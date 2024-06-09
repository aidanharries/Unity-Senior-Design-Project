using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject[] shipPrefabs;
    public GameObject[] healthBarPrefabs;
    public Transform healthCanvas;
    private GameObject ship;
    private GameObject healthBar;

    void Start()
    {
        // Get the main camera's position
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 spawnPosition = new Vector3(cameraPos.x, cameraPos.y - 2, 0);
        int index = ShipSelect.SelectedShipIndex;

        if (index >= 0 && index < shipPrefabs.Length)
        {
            ship = Instantiate(shipPrefabs[index], spawnPosition, Quaternion.identity);

            // Instantiate the corresponding health bar
            if (index < healthBarPrefabs.Length)
            {
                healthBar = Instantiate(healthBarPrefabs[index], new Vector3(200, 65, 0), Quaternion.identity);
                healthBar.transform.SetParent(healthCanvas, false);
                healthBar.transform.localScale = new Vector3(100, 100, 1);
                ship.GetComponent<Player>().playerHealth = healthBar.GetComponent<PlayerHealth>();
            }
        }
    }
}
