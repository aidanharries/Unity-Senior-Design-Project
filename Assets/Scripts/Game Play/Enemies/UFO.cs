using UnityEngine;

public class UFO : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Bullet bulletPrefab;
    public float shootingInterval = 2f;
    public float circlingRadius = 5f;
    public float circlingSpeed = 2f;
    public SpriteRenderer explosionRenderer;
    public Sprite[] explosionFrames;
    public float frameRate = 0.1f;
    public GameObject resourcePrefab;
    public AudioClip shootingSound;
    public AudioClip explosionSound;

    private Rigidbody2D rb;
    private float shootingTimer;
    private bool isExploding = false;
    private int currentFrame;
    private float frameTimer;
    private WaveSpawner waveSpawner;
    private Vector2 centerPoint;
    private bool isCircling = false;
    private bool firstShot = true;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        waveSpawner = FindObjectOfType<WaveSpawner>();
        shootingTimer = shootingInterval;
        centerPoint = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isExploding)
        {
            ExplosionAnimation();
            return;
        }

        if (!isCircling)
        {
            MoveTowardsCenter();
        }
        else
        {
            CircleAroundCenter();
            OrientTowardsCenter();
            if(firstShot) 
            {
                Shoot();
                firstShot = false;
            }
            HandleShooting();
        }
    }

    private void MoveTowardsCenter()
    {
        Vector2 direction = (centerPoint - (Vector2)transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        if (Vector2.Distance(transform.position, centerPoint) <= circlingRadius)
        {
            isCircling = true;
            rb.velocity = Vector2.zero; // Stop linear movement
        }
    }

    private void CircleAroundCenter()
    {
        transform.RotateAround(centerPoint, Vector3.forward, circlingSpeed * Time.deltaTime);
    }

    private void OrientTowardsCenter()
    {
        // Calculate the angle towards the center
        Vector2 directionToCenter = centerPoint - (Vector2)transform.position;
        float angle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90)); // Adjust for sprite orientation
    }

    private void HandleShooting()
    {
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
        Vector2 shootDirection = (centerPoint - (Vector2)transform.position).normalized;
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
                ScoreManager.Instance.AddScore(100); // Add 100 points for shooting a UFO
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
}
