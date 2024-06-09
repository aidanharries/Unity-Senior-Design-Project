using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float speed = 1f; // speed of the asteroid's movement
    public float rotationSpeed = 50f; // speed of the asteroid's rotation
    public SpriteRenderer explosionRenderer;
    public Sprite[] explosionFrames;
    public float frameRate = 0.1f;
    public GameObject resourcePrefab;
    public AudioClip explosionSound;

    private int currentFrame;
    private float frameTimer;
    private bool hasEnteredViewport = false;
    private WaveSpawner waveSpawner;
    private bool isExploding = false;
    private SpriteRenderer spriteRenderer; // The main sprite renderer for the asteroid
    private GameObject explosionObject;
    private AudioSource audioSource;

    private void Awake()
    {
        waveSpawner = FindObjectOfType<WaveSpawner>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // move towards the center of the screen
        Vector3 centerOfScreen = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        centerOfScreen.z = 0;
        Vector3 directionToCenter = (centerOfScreen - transform.position).normalized;

        // apply the movement
        GetComponent<Rigidbody2D>().velocity = directionToCenter * speed;

        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
    }

    private void Update()
    {
        // Check if the asteroid is within the viewport
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPosition.x > 0 && viewportPosition.x < 1 && viewportPosition.y > 0 && viewportPosition.y < 1)
        {
            hasEnteredViewport = true;
        }

        // If the asteroid has left the viewport, destroy it
        if (hasEnteredViewport && (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1))
        {
            Destroy(gameObject);
            waveSpawner.EnemyDestroyed();
        }

        // apply rotation
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // check for explosion
        if (isExploding)
        {
            ExplosionAnimation();
        }
    }

    private void ExplosionAnimation()
    {
        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer -= frameRate;
            currentFrame++;

            if (currentFrame < explosionFrames.Length)
            {
                explosionRenderer.sprite = explosionFrames[currentFrame];
            }
            else
            {
                // Explosion animation finished
                Destroy(explosionRenderer.gameObject); // Destroy the explosion GameObject
                DestroyAsteroidAfterExplosion();
            }
        }
    }

    private void DestroyAsteroidAfterExplosion()
    {
        Destroy(gameObject); // Destroy the asteroid GameObject
        waveSpawner.EnemyDestroyed(); // Notify the spawner that the asteroid has been destroyed
        isExploding = false; // Reset the flag
    }

    private void StartExplosion()
    {
        // Disable the main asteroid sprite and stop movement and rotation
        spriteRenderer.enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Disable the collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Detach the explosion renderer and set its position
        explosionRenderer.transform.SetParent(null);
        explosionRenderer.transform.position = transform.position;
        explosionObject = explosionRenderer.gameObject; // Store the reference

        // Start the explosion animation
        isExploding = true;
        audioSource.PlayOneShot(explosionSound); // Play the explosion sound
        explosionRenderer.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isExploding)
        {
            if (collision.CompareTag("Laser"))
            {
                Destroy(collision.gameObject); // Destroy the laser
                StartExplosion();
                DropResource();
                ScoreManager.Instance.AddScore(100); // Add 100 points for shooting an asteroid
            }
            else if (collision.CompareTag("Ship"))
            {
                StartExplosion();
            }
            else if (collision.CompareTag("Planet"))
            {
                StartExplosion();
            }
            else if (collision.CompareTag("Moon"))
            {
                StartExplosion();
                DropResource();
                ScoreManager.Instance.AddScore(100);
            }
        }
    }

    private void DropResource()
    {
        int multiplier = GlobalResourceMultiplier.Instance.CurrentMultiplier;
        for (int i = 0; i < multiplier; i++)
        {
            Instantiate(resourcePrefab, transform.position, Quaternion.identity);
        }
    }
}
