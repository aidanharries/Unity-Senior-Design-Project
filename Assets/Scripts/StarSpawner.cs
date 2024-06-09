using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    public GameObject starPrefab;
    public int starCount = 100;
    public float starSize = 1.0f;
    private Vector2 screenBounds;

    void Start()
    {
        // Calculate screen bounds based on the main camera's properties
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float camHeight = Camera.main.orthographicSize * 2;
        screenBounds = new Vector2(camHeight * screenAspect, camHeight) / 2;

        for (int i = 0; i < starCount; i++)
        {
            GameObject star = Instantiate(starPrefab, transform);
            float posX = Random.Range(-screenBounds.x, screenBounds.x);
            float posY = Random.Range(-screenBounds.y, screenBounds.y);
            star.transform.position = new Vector3(posX, posY, 0);
            star.transform.localScale = Vector3.one * starSize;
        }
    }
}
