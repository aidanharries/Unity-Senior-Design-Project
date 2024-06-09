using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonBehavior : MonoBehaviour
{
    public float orbitRadius = 5f;
    public float orbitSpeed = 0f;
    private Transform planetTransform; // Reference to the planet's transform

    private void Awake()
    {
        // Check if the moon has a parent
        if (transform.parent != null)
        {
            planetTransform = transform.parent.transform;
            PositionMoonOnOrbit();
        }
        else
        {
            Debug.LogError("Moon is not a child of any planet!");
        }
        orbitSpeed = 0f;
    }

    private void Update()
    {
        if (planetTransform != null)
        {
            OrbitAroundPlanet();
        }
    }

    public void IncreaseOrbitSpeed()
    {
        orbitSpeed += 25f;
    }

    private void PositionMoonOnOrbit()
    {
        // Set the initial position of the moon in the orbit
        transform.localPosition = Vector3.right * orbitRadius;
    }

    private void OrbitAroundPlanet()
    {
        // Rotate around the planet
        transform.RotateAround(planetTransform.position, Vector3.forward, orbitSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyLaser"))
        {
            Destroy(collision.gameObject);
        }
    }
}

