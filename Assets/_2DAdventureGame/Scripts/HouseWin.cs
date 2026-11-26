using UnityEngine;

public class HouseWin : MonoBehaviour
{
    public int requiredHealthGained = 3;
    private bool playerReached = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerReached)
            return;

        RobotController player = other.GetComponentInParent<RobotController>();

        if (player != null)
        {
            if (player.HealthGainedTotal >= requiredHealthGained)
            {
                playerReached = true;
                player.TriggerEndGame(true);
            }
            else
            {
                Debug.Log("Треба зібрати 3 аптечки! Зараз зібрано: " + player.HealthGainedTotal);
            }
        }
    }
}