using UnityEngine;

public class Homing : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationSpeed = 4f;
    public Bullet bulletPrefab;
    public float shootingInterval = 2f;
    public SpriteRenderer explosionRenderer;
    public Sprite[] explosionFrames;
    public float frameRate = 0.1f;
    public GameObject resourcePrefab;
    public AudioClip shootingSound;
    public AudioClip explosionSound;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private float shootingTimer;
    private bool isExploding = false;
    private int currentFrame;
    private float frameTimer;
    private WaveSpawner waveSpawner;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        waveSpawner = FindObjectOfType<WaveSpawner>();
        shootingTimer = shootingInterval;
        UpdatePlayerTransform();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isExploding)
        {
            ExplosionAnimation();
            return;
        }

        if (playerTransform == null)
        {
            UpdatePlayerTransform();
            if (playerTransform == null)
                return; // No player found
        }

        // Movement towards player
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        // Rotation towards player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        // Shooting logic
        shootingTimer -= Time.deltaTime;
        if (shootingTimer <= 0)
        {
            Shoot();
            shootingTimer = shootingInterval;
        }
    }

    private void Shoot()
    {
        // Instantiate the bullet and set its direction
        Bullet bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.Project(transform.up);

        audioSource.PlayOneShot(shootingSound); // Play the shooting sound
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
                Destroy(gameObject); // Destroy the Homing GameObject
                waveSpawner.EnemyDestroyed(); // Notify the spawner that the homing ship has been destroyed
                isExploding = false; // Reset the flag
            }
        }
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
                ScoreManager.Instance.AddScore(100); // Add 100 points for shooting a homing ship
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

    private void StartExplosion()
    {
        // Disable the main sprite and stop movement and rotation
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        GetComponent<SpriteRenderer>().enabled = false;

        // Disable the collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Start the explosion animation
        isExploding = true;
        audioSource.PlayOneShot(explosionSound); // Play the explosion sound
        explosionRenderer.enabled = true;
    }

    private void UpdatePlayerTransform()
    {
        GameObject playerObject = GameObject.FindWithTag("Ship");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }
}
