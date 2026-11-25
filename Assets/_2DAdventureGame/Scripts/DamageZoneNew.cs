
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerStay2D(Collider2D other)
    {
        RobotController controller =
            other.GetComponentInParent<RobotController>();

        if (controller != null)
        {
            controller.ChangeHealth(-damage);
        }
    }
}
