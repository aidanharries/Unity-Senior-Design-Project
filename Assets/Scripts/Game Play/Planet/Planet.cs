using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Planet : MonoBehaviour
{
    public PlanetHealth planetHealth;
    public Sprite[] damageFlashSprites;
    public SpriteRenderer explosionRenderer;
    public Sprite[] explosionFrames;
    public float frameRate = 0.1f; 
    public AudioClip explosionSound;
    public AudioClip planetHurt;

    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;
    private int currentFrame;
    private float frameTimer;
    private bool isExploding = false;
    private int turretsEnabled = 0;
    private AudioSource audioSource;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child != null && (child.CompareTag("DefenseTurret") || child.CompareTag("Moon")))
            {
                child.SetActive(false); // Disable the child GameObject
            }
        }
        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
    }

    private void Update()
    {
        if (isExploding)
        {
            return;
        }

        // Play explosion when health is zero
        if (planetHealth.health <= 0 && !isExploding)
        {
            StartCoroutine(ExplosionRoutine());
        }
    }

    public void AddTurret()
    {
        int turretsToEnable = 2; // Number of turrets to enable

        while (turretsEnabled < transform.childCount && turretsToEnable > 0)
        {
            GameObject child = transform.GetChild(turretsEnabled).gameObject;
            if (child != null && child.CompareTag("DefenseTurret"))
            {
                child.SetActive(true); // Enable the child 
                turretsToEnable--; // Decrement the number of turrets left to enable
            }
            turretsEnabled++; // Increment the count of checked objects

            // Check if all required turrets for this call have been enabled
            if (turretsToEnable <= 0)
            {
                break; // Exit the loop after enabling the required number of turrets
            }
        }
    }

    private void AddMoon()
    {
        GameObject child = transform.GetChild(9).gameObject;
        if (child != null && child.CompareTag("Moon"))
        {
            child.SetActive(true);
        }
    }

    public void IncreaseMoonSpeed()
    {
        GameObject child = transform.GetChild(9).gameObject;
        if (child != null && child.CompareTag("Moon"))
        {
            // Activate the moon if it's not active
            if (!child.activeInHierarchy)
            {
                child.SetActive(true);
            }

            // Get the MoonBehavior component and increase the orbit speed
            MoonBehavior moonBehavior = child.GetComponent<MoonBehavior>();
            if (moonBehavior != null)
            {
                moonBehavior.IncreaseOrbitSpeed();
            }
            else
            {
                Debug.LogError("MoonBehavior component not found on the moon GameObject!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            planetHealth.ChangeHealth(-10f);
            StartCoroutine(FlashEffect());
        }
        else if (collision.CompareTag("EnemyLaser"))
        {
            planetHealth.ChangeHealth(-10f);
            Destroy(collision.gameObject); // Destroy the laser
            StartCoroutine(FlashEffect());
        }
    }

    private IEnumerator FlashEffect()
    {
        audioSource.PlayOneShot(planetHurt); // Play the explosion sound

        // Iterate over each sprite in the damageFlashSprites array
        foreach (Sprite flashSprite in damageFlashSprites)
        {
            // Change to the flash sprite
            spriteRenderer.sprite = flashSprite;
            // Wait for frameRate seconds
            yield return new WaitForSeconds(frameRate);
        }

        // Return to the original sprite
        spriteRenderer.sprite = originalSprite;

        if (planetHealth.health <= 0)
        {
            yield break; 
        }
    }

    private IEnumerator ExplosionRoutine()
    {
        isExploding = true;

        audioSource.PlayOneShot(explosionSound); // Play the explosion sound

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child != null && child.CompareTag("DefenseTurret"))
            {
                child.SetActive(false); // Disable the child GameObject
            }
        }

        explosionRenderer.enabled = true;
        
        for (int i = 0; i < explosionFrames.Length; i++)
        {
            explosionRenderer.sprite = explosionFrames[i];
            yield return new WaitForSeconds(frameRate);
        }

        SceneManager.LoadScene("GameOver");
    }
}
