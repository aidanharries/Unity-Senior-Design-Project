using UnityEngine;

public class StarParallax : MonoBehaviour
{
    public GameObject starsFrontPrefab;
    public GameObject starsMiddlePrefab;
    public GameObject starsBackPrefab;

    private GameObject starsFront;
    private GameObject starsMiddle;
    private GameObject starsBack;

    public float frontParallaxStrength = 0.3f;
    public float middleParallaxStrength = 0.2f;
    public float backParallaxStrength = 0.09f;

    private Vector2 _thrustDirection;
    private Player _playerScript;

    public GameObject planet;
    public GameObject healthBarPrefab;
    private GameObject healthBar;
    public float planetParallaxStrength = 0.5f;
    private GameObject planetInstance;

    public Transform healthCanvas;

    private void Start()
    {
        // Get the main camera's position
        Vector3 cameraPos = Camera.main.transform.position;
        
        Vector3 starPosition = new Vector3(cameraPos.x, cameraPos.y, 0);
        
        // Instantiate the star layers at the camera's position
        starsFront = Instantiate(starsFrontPrefab, starPosition, Quaternion.identity);
        starsMiddle = Instantiate(starsMiddlePrefab, starPosition, Quaternion.identity);
        starsBack = Instantiate(starsBackPrefab, starPosition, Quaternion.identity);

        _playerScript = GetComponent<Player>();

        // Instantiate planet and health bar
        planetInstance = Instantiate(planet, starPosition, Quaternion.identity);
        healthBar = Instantiate(healthBarPrefab, new Vector3(200, 175, 0), Quaternion.identity);

        // Find the Health Canvas by Tag
        Transform healthCanvasTransform = GameObject.FindGameObjectWithTag("HealthCanvas").transform;

        // Set the Health Canvas as the parent and adjust scale
        healthBar.transform.SetParent(healthCanvasTransform, false);
        healthBar.transform.localScale = new Vector3(100, 100, 1);

        // Dynamically assign the PlanetHealth component to the Planet script
        Planet planetScript = planetInstance.GetComponent<Planet>();
        if (planetScript != null)
        {
            planetScript.planetHealth = healthBar.GetComponent<PlanetHealth>();
        }
    }

    private void Update()
    {
        if (_playerScript != null)
        {
            _thrustDirection = _playerScript.IsThrusting() ? (Vector2)_playerScript.transform.up : Vector2.zero;
            MoveStars();
            MovePlanet();
        }
    }

    private void MoveStars()
    {
        if (starsFront != null)
        {
            starsFront.transform.localPosition = Vector3.Lerp(starsFront.transform.localPosition, _thrustDirection * frontParallaxStrength, Time.deltaTime);
        }
        if (starsMiddle != null)
        {
            starsMiddle.transform.localPosition = Vector3.Lerp(starsMiddle.transform.localPosition, _thrustDirection * middleParallaxStrength, Time.deltaTime);
        }
        if (starsBack != null)
        {
            starsBack.transform.localPosition = Vector3.Lerp(starsBack.transform.localPosition, _thrustDirection * backParallaxStrength, Time.deltaTime);
        }
    }

    private void MovePlanet()
    {
        if (planetInstance != null)
        {
            planetInstance.transform.localPosition = Vector3.Lerp(planetInstance.transform.localPosition, _thrustDirection * planetParallaxStrength, Time.deltaTime);
        }
    }
}
