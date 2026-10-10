
using UnityEngine;
using UnityEngine.InputSystem;

public class RobotController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;
    private int currentHealth;

    public int health
    {
        get { return currentHealth; }
    }

    public float timeInvincible = 2.0f;
    private bool isInvincible;
    private float damageCooldown;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        Debug.Log("ROBOT CONTROLLER ПРАЦЮЄ");
    }

    void Update()
    {
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;

            if (damageCooldown <= 0)
            {
                isInvincible = false;
            }
        }

        moveInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                moveInput.y = 1;

            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                moveInput.y = -1;

            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveInput.x = -1;

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveInput.x = 1;
        }

        moveInput = moveInput.normalized;
    }

    void FixedUpdate()
    {
        Vector2 targetPosition = rb.position +
            moveInput * speed * Time.fixedDeltaTime;

        rb.MovePosition(targetPosition);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            damageCooldown = timeInvincible;
        }

        currentHealth = Mathf.Clamp(
            currentHealth + amount,
            0,
            maxHealth
        );

        Debug.Log("Поточне здоров'я: " + currentHealth + "/" + maxHealth);
    }
}
