
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.linearDamping = 0;
    }

    public void Launch(Vector2 direction, float force)
    {
        rb.gravityScale = 0;
        rb.linearVelocity = direction.normalized * force;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();

        if (enemy != null)
        {
            enemy.TakeDamage(1);
            Destroy(gameObject);
            return;
        }

        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (transform.position.magnitude > 100f)
        {
            Destroy(gameObject);
        }
    }
}
