
using UnityEngine;

public class DamageZoneNew : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.ChangeHealth(-1);
        }
    }
}
