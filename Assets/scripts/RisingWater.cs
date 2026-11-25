
using UnityEngine;

public class RisingWater : MonoBehaviour
{
    public float riseSpeed = 0.25f;
    public float maxHeight = 4.5f;
    public float startDelay = 2f;

    private float delayTimer;
    private bool isTriggered;

    void Update()
    {
        if (isTriggered)
            return;

        if (delayTimer < startDelay)
        {
            delayTimer += Time.deltaTime;
            return;
        }

        if (transform.position.y < maxHeight)
        {
            transform.Translate(
                Vector3.up * riseSpeed * Time.deltaTime
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered)
            return;

        RobotController player =
            other.GetComponentInParent<RobotController>();

        if (player != null)
        {
            isTriggered = true;
            player.TriggerEndGame(false);
        }
    }
}
