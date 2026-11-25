
using UnityEngine;

public class FoodPickup : MonoBehaviour
{
    public int healthAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        RobotController player =
            other.GetComponentInParent<RobotController>();

        if (player == null)
            return;

        Debug.Log("ЇЖА: додаємо здоров'я!");

        player.ChangeHealth(Mathf.Abs(healthAmount));

        Destroy(gameObject);
    }
}
