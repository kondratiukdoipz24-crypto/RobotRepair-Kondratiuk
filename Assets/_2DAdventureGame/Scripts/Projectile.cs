
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController enemy =
            other.GetComponentInParent<EnemyController>();

        if (enemy != null)
        {
            enemy.TakeDamage(1);
            Destroy(gameObject);
            return;
        }

        if (other.GetComponentInParent<RobotController>() != null)
            return;

        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
