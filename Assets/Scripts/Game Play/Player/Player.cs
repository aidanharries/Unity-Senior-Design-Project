using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public float thrustSpeed = 9.0f;
    public float turnSpeed = 1.0f;
    public Bullet bulletPrefab;
    public float mouseRotationSpeed = 2.0f;  
    public static float shootTimerMax = 1.2f;
    public float shootTimer = 1.2f;
    public float enemyDamage = 10f;
    public SpriteRenderer[] thrusterRenderers;
    public Sprite[] thrusterFrames;
    public float frameRate = 0.1f; 
    public PlayerHealth playerHealth;
    public Sprite[] damageFlashSprites;
    public Sprite originalSprite;
    public SpriteRenderer explosionRenderer;
    public Sprite[] explosionFrames;
    public AudioClip shootingSound;
    public AudioClip explosionSound;
    public AudioClip playerHurt;
    public AudioClip scrap;

    private static float originalThrustSpeed = 5f;
    private static float originalShootTimerMax = 1.2f;
    private static float originalEnemyDamage = 10f;
    private Rigidbody2D _rigidbody;
    private bool _thrusting;
    private float _turnDirection;
    private Quaternion _targetRotation;
    private bool _usingMouseInput = false;
    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;
    private bool _holdingShootButton = false;
    private int currentFrame;
    private float frameTimer;
    private bool isExploding = false;
    private AudioSource audioSource;

    private void Awake()
    {
        ResetSpeedToOriginal();

        _rigidbody = GetComponent<Rigidbody2D>();
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        objectWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x; // Gets the width half of the object
        objectHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y; // Gets the height half of the object
        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
    }

    // Method to increase the thrust speed
    public void IncreaseThrustSpeed(float increment)
    {
        thrustSpeed += increment;
        Debug.Log("Thrust Speed increased to: " + thrustSpeed);
    }

    // Method to decrease the shoot timer
    public void DecreaseShootTimer(float decrement)
    {
        shootTimerMax = Mathf.Max(0.1f, shootTimerMax - decrement);
        Debug.Log("Shoot Timer decreased to: " + shootTimerMax);
    }

    // Mehtod to decrease the enemy damage
    public void DecreaseEnemyDamage(float decrement)
    {
        enemyDamage -= decrement;
        Debug.Log("Enemy Damage decreased to: " + enemyDamage);
    }

    // Method to reset speed to original
    public void ResetSpeedToOriginal()
    {
        thrustSpeed = originalThrustSpeed;
        shootTimerMax = originalShootTimerMax;
        enemyDamage = originalEnemyDamage;
    }

    void Update()
    {
        if (isExploding)
        {
            return;
        }

        if(shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }

        // Check for auto-fire while holding the button
        if (_holdingShootButton && shootTimer <= 0)
        {
            Shoot();
        }

        // Play explosion when health is zero
        if (playerHealth.health <= 0 && !isExploding)
        {
            StartCoroutine(ExplosionRoutine());
        }
    }

    private void FixedUpdate()
    {
        if (isExploding)
        {
            return;
        }

        if (_thrusting)
        {
            _rigidbody.AddForce(transform.up * thrustSpeed);
            AnimateThrusters();
        }
        else
        {
            HideThrusters();
        }

        if (!_usingMouseInput && _turnDirection != 0.0f)
        {
            _rigidbody.AddTorque(_turnDirection * turnSpeed);
        }
        
        if (_usingMouseInput)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, mouseRotationSpeed * Time.deltaTime);
        }

        ScreenWrap();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Resource"))
        {
            CollectResource(collision.gameObject);
            ResourceManager.Instance.AddCount(1); // Add 1 count for collecting the resource
            ScoreManager.Instance.AddScore(25); // Add 25 points for collecting the resource
        }
        else if (collision.CompareTag("Enemy"))
        {
            playerHealth.ChangeHealth(-enemyDamage);
            StartCoroutine(DamageFlashRoutine());
        }
        else if (collision.CompareTag("EnemyLaser"))
        {
            playerHealth.ChangeHealth(-enemyDamage);
            Destroy(collision.gameObject); // Destroy the laser
            StartCoroutine(DamageFlashRoutine());
        }
    }

    private void CollectResource(GameObject resource)
    {
        audioSource.PlayOneShot(scrap); 
        
        Destroy(resource);
    }

    private IEnumerator DamageFlashRoutine()
    {
        audioSource.PlayOneShot(playerHurt); 

        // Save the original sprite if not already saved
        if (originalSprite == null)
        {
            originalSprite = GetComponent<SpriteRenderer>().sprite;
        }

        // Flash the damage sprites
        foreach (var flashSprite in damageFlashSprites)
        {
            GetComponent<SpriteRenderer>().sprite = flashSprite;
            yield return new WaitForSeconds(0.1f);
        }

        // Return to the original sprite
        GetComponent<SpriteRenderer>().sprite = originalSprite;

        if (playerHealth.health <= 0)
        {
            yield break; 
        }
    }

    private IEnumerator ExplosionRoutine()
    {
        isExploding = true;

        audioSource.PlayOneShot(explosionSound); // Play the explosion sound

        _thrusting = false;
        _turnDirection = 0f;

        HideThrusters();

        GetComponent<SpriteRenderer>().enabled = false;

        // Stop all movement and rotation
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.freezeRotation = true;

        _rigidbody.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;

        explosionRenderer.enabled = true;
        
        for (int i = 0; i < explosionFrames.Length; i++)
        {
            explosionRenderer.sprite = explosionFrames[i];
            yield return new WaitForSeconds(frameRate);
        }

        SceneManager.LoadScene("GameOver");
    }

    private void AnimateThrusters()
    {
        frameTimer += Time.fixedDeltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer -= frameRate;
            currentFrame = (currentFrame + 1) % thrusterFrames.Length;
            foreach (var renderer in thrusterRenderers)
            {
                renderer.sprite = thrusterFrames[currentFrame];
            }
        }
    }

    private void HideThrusters()
    {
        foreach (var renderer in thrusterRenderers)
        {
            renderer.sprite = null;
        }
    }

    public void OnThrust(InputValue value)
    {
        if (isExploding) return;

        Vector2 thrustInput = value.Get<Vector2>();
        if (thrustInput.y > 0)
        {
            _thrusting = true;
        }
        else
        {
            _thrusting = false;
        }
    }

    public void OnGamepadThrust(InputValue value)
    {
        if (isExploding) return;

        float thrustInput = value.Get<float>();
        if (thrustInput > 0.1f) 
        {
            _thrusting = true;
        }
        else
        {
            _thrusting = false;
        }
    }

    public void OnRotate(InputValue value)
    {
        if (isExploding) return;

        _usingMouseInput = false;  // reset the flag

        Vector2 rotateInput = value.Get<Vector2>();
        _turnDirection = -rotateInput.x;
    }

    public void OnShoot(InputValue value)
    {
        if (isExploding) return;

        _holdingShootButton = value.isPressed;
    }

    private void Shoot()
    {
        if(shootTimer <= 0)
        {
            Vector3 bulletSpawnOffset = transform.up * 0.5f; 
        
            Bullet bullet = Instantiate(bulletPrefab, transform.position + bulletSpawnOffset, transform.rotation);
            bullet.Project(transform.up);

            audioSource.PlayOneShot(shootingSound); // Play the shooting sound

            shootTimer = shootTimerMax;
        }
    }

    public void OnMousePosition(InputValue value)
    {
        if (isExploding) return;

        _usingMouseInput = true;

        Vector2 mousePosition = value.Get<Vector2>();
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, transform.position.z));
        Vector3 direction = worldMousePosition - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        _targetRotation = Quaternion.Euler(0, 0, angle);
    }

    public void OnMouseThrust(InputValue value)
    {
        if (isExploding) return;

        if (value.isPressed)
        {
            _thrusting = true;
        }
        else
        {
            _thrusting = false;
        }
    }

    public bool IsThrusting()
    {
        return _thrusting;
    }

    private void ScreenWrap()
    {
        Vector3 newPosition = transform.position;

        if (transform.position.x > screenBounds.x + objectWidth)
        {
            newPosition.x = -screenBounds.x - objectWidth;
        }

        if (transform.position.x < -screenBounds.x - objectWidth)
        {
            newPosition.x = screenBounds.x + objectWidth;
        }

        if (transform.position.y > screenBounds.y + objectHeight)
        {
            newPosition.y = -screenBounds.y - objectHeight;
        }

        if (transform.position.y < -screenBounds.y - objectHeight)
        {
            newPosition.y = screenBounds.y + objectHeight;
        }

        transform.position = newPosition;
    }
}
