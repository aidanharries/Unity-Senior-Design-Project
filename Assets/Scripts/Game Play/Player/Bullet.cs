using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    public float speed = 500.0f;
    public float maxLifetime = 10.0f;

    private void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Check if the bullet is outside the screen bounds
        CheckForOutOfBounds();
    }

    public void Project(Vector2 direction)
    {
        _rigidbody.AddForce(direction * this.speed);
        Destroy(this.gameObject, this.maxLifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }

    private void CheckForOutOfBounds()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if(viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
        {
            Destroy(this.gameObject);
        }
    }
}