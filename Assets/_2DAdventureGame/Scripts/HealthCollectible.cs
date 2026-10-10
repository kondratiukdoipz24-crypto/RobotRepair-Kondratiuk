
using UnityEngine;

public class HealthCollectibleNew : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health != null && health.health < health.maxHealth)
        {
            health.ChangeHealth(1);

            Debug.Log("Їжа: +1 HP");

            Destroy(gameObject);
        }
    }
}
