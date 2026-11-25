
using UnityEngine;

public class HouseWin : MonoBehaviour
{
    private bool playerReached = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerReached)
            return;

        RobotController player =
            other.GetComponentInParent<RobotController>();

        if (player != null)
        {
            playerReached = true;
            player.TriggerEndGame(true);
        }
    }
}
