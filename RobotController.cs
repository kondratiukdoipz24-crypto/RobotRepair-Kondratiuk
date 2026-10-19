
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

    public GameObject projectilePrefab;
    public float projectileForce = 300f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.right;

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
                isInvincible = false;
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

           if (Keyboard.current.spaceKey.wasPressedThisFrame)
{
    if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        lastMoveDirection = Vector2.up;
    else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        lastMoveDirection = Vector2.down;
    else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        lastMoveDirection = Vector2.left;
    else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        lastMoveDirection = Vector2.right;

    Launch();
}
        }

        moveInput = moveInput.normalized;

        if (moveInput.x != 0 || moveInput.y != 0)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                lastMoveDirection = moveInput.x > 0 ? Vector2.right : Vector2.left;
            else
                lastMoveDirection = moveInput.y > 0 ? Vector2.up : Vector2.down;
        }
    }

   void FixedUpdate()
{
    if (rb == null)
        return;

    Vector2 targetPosition = rb.position
        + moveInput * speed * Time.fixedDeltaTime;

    rb.MovePosition(targetPosition);
}

    void Launch()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Не призначено Projectile Prefab!");
            return;
        }

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            rb.position + lastMoveDirection * 0.5f,
            Quaternion.identity
        );

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile != null)
            projectile.Launch(lastMoveDirection, projectileForce);
        else
        {
            Debug.LogError("На снаряді немає скрипта Projectile!");
            Destroy(projectileObject);
        }
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

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        Debug.Log("Поточне здоров'я: " + currentHealth + "/" + maxHealth);
    }
}
