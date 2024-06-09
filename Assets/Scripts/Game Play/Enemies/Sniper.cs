using UnityEngine;

public class Sniper : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float stopDistance = 5f;  // Distance from the player to stop and start shooting
    public float damping = 0.25f;     // Damping effect as sniper approaches the player
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
    private bool firstShot = true;
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

        RotateTowardsPlayer(); // rotation towards player
        MoveTowardsPlayer(Vector2.Distance(transform.position, playerTransform.position));

        if (IsWithinStopDistance())
        {
            rb.velocity = Vector2.zero; // Ensure complete stop
            if(firstShot) 
            {
                Shoot();
                firstShot = false;
            }
            ShootAtPlayer();
        }
    }

    private bool IsWithinStopDistance()
    {
        return Vector2.Distance(transform.position, playerTransform.position) <= stopDistance + 0.15f;
    }

    private void RotateTowardsPlayer()
    {
        Vector2 direction = playerTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void MoveTowardsPlayer(float distanceToPlayer)
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        float speedModifier = Mathf.Clamp01((distanceToPlayer - stopDistance) / damping);
        rb.velocity = direction * moveSpeed * speedModifier;
    }

    private void ShootAtPlayer()
    {
        // Decrement the shooting timer regardless of the distance
        shootingTimer -= Time.deltaTime;
        if (shootingTimer <= 0 )
        {
            Shoot();
            shootingTimer = shootingInterval; // Reset the timer
        }
    }

    private void Shoot()
    {
        Vector2 shootDirection = (playerTransform.position - transform.position).normalized;
        Quaternion shootRotation = Quaternion.LookRotation(Vector3.forward, shootDirection);
        Bullet bullet = Instantiate(bulletPrefab, transform.position, shootRotation);
        bullet.Project(shootDirection);

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
