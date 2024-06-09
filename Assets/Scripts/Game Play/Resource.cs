using UnityEngine;

public class Resource : MonoBehaviour
{
    public float initialSpeed = 5f;
    public float deceleration = 10f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Give the resource a random direction
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        rb.velocity = randomDirection * initialSpeed;

        // Destroy the resource after 5 seconds
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        // Slow down the resource over time
        rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, deceleration * Time.deltaTime);
    }
}
