
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private RobotController robot;

    public int maxHealth
    {
        get
        {
            return robot != null ? robot.maxHealth : 0;
        }
    }

    public int health
    {
        get
        {
            return robot != null ? robot.health : 0;
        }
    }

    private void Awake()
    {
        robot = GetComponent<RobotController>();

        if (robot == null)
        {
            Debug.LogError("Не знайдено RobotController на роботі!");
        }
    }

    public void ChangeHealth(int amount)
    {
        if (robot != null)
        {
            robot.ChangeHealth(amount);
        }
    }
}
