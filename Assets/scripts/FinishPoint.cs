using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        RobotController player = other.GetComponentInParent<RobotController>();

        if (player != null)
        {
            // Перевіряємо, чи всі Health Collection зібрано
            if (HealthCollectionManager.Instance != null && HealthCollectionManager.Instance.AreAllCollected())
            {
                player.TriggerEndGame(true); // Обидві умови виконано -> Перемога!
            }
            else
            {
                Debug.Log("Ще не всі предмети зібрано! Повернися і дозбирай.");
            }
        }
    }
}