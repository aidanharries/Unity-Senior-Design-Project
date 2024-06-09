using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseTurretShoot : MonoBehaviour
{
    public Bullet bulletPrefab;
    public float shootTimer = 5f;
    public float shootTimerMax = 5f;
    public AudioClip shootingSound; 
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
    }

    void Update()
    {
        if(shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }

        if (shootTimer <= 0)
        {
            Shoot();
        }
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
}
