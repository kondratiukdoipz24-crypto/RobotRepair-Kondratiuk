
using UnityEngine;
using UnityEngine.InputSystem;

public class RobotController : MonoBehaviour
{
    public float speed = 3f;

    public int maxHealth = 5;
    private int currentHealth;

    public int health
    {
        get { return currentHealth; }
    }

    private NonPlayerCharacter lastNonPlayerCharacter;

    public float timeInvincible = 2f;
    private bool isInvincible;
    private float invincibleTimer;

    public GameObject projectilePrefab;
    public float projectileForce = 10f;
    public float projectileSpawnDistance = 0.8f;

    public InputAction TalkAction;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        if (rb == null)
            Debug.LogError("На роботі немає Rigidbody2D!");

        if (animator == null)
            Debug.LogError("На роботі немає Animator!");

        if (UIHandler.instance != null)
        {
            UIHandler.instance.SetHealthValue(
                currentHealth / (float)maxHealth
            );
        }
        else
        {
            Debug.LogError("UIHandler не знайдено!");
        }

        if (TalkAction != null)
            TalkAction.Enable();
        else
            Debug.LogError("TalkAction не налаштовано!");
    }

    void Update()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed ||
                Keyboard.current.aKey.isPressed)
                moveInput.x = -1f;

            if (Keyboard.current.rightArrowKey.isPressed ||
                Keyboard.current.dKey.isPressed)
                moveInput.x = 1f;

            if (Keyboard.current.downArrowKey.isPressed ||
                Keyboard.current.sKey.isPressed)
                moveInput.y = -1f;

            if (Keyboard.current.upArrowKey.isPressed ||
                Keyboard.current.wKey.isPressed)
                moveInput.y = 1f;

            moveInput = moveInput.normalized;

            if (moveInput != Vector2.zero)
                lastMoveDirection = moveInput;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                Launch();
        }

        if (animator != null)
        {
            animator.SetFloat("Look X", lastMoveDirection.x);
            animator.SetFloat("Look Y", lastMoveDirection.y);
            animator.SetFloat("Speed", moveInput.sqrMagnitude);
        }

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;

            if (invincibleTimer <= 0f)
                isInvincible = false;
        }

        // Перевірка, чи робот дивиться на NPC
        if (rb != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                rb.position + Vector2.up * 0.2f,
                lastMoveDirection,
                1.5f,
                LayerMask.GetMask("NPC")
            );

            if (hit.collider != null)
            {
                NonPlayerCharacter npc =
                    hit.collider.GetComponent<NonPlayerCharacter>();

                if (npc != null)
                {
                    if (lastNonPlayerCharacter != null &&
                        lastNonPlayerCharacter != npc)
                    {
                        if (lastNonPlayerCharacter.dialogueBubble != null)
                            lastNonPlayerCharacter.dialogueBubble.SetActive(false);
                    }

                    if (npc.dialogueBubble != null)
                        npc.dialogueBubble.SetActive(true);

                    lastNonPlayerCharacter = npc;

                    FindFriend();
                }
                else
                {
                    HideLastDialogueBubble();
                }
            }
            else
            {
                HideLastDialogueBubble();
            }
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        rb.linearVelocity = Vector2.zero;

        if (moveInput != Vector2.zero)
        {
            rb.MovePosition(
                rb.position + moveInput * speed * Time.fixedDeltaTime
            );
        }
    }

    public void ChangeHealth(int amount)
{
    Debug.Log("ChangeHealth викликано. Отримано: " + amount);

    if (amount < 0 && isInvincible)
    {
        Debug.Log("Робот невразливий!");
        return;
    }

    currentHealth = Mathf.Clamp(
        currentHealth + amount, 0, maxHealth
    );

    Debug.Log("Здоров'я: " + currentHealth + "/" + maxHealth);

    if (UIHandler.instance != null)
    {
        float healthPercent = (float)currentHealth / maxHealth;

        UIHandler.instance.SetHealthValue(healthPercent);

        Debug.Log("Передано повзунку: " + healthPercent);
    }
    else
    {
        Debug.LogError("UIHandler.instance дорівнює null!");
    }

    if (amount < 0)
    {
        isInvincible = true;
        invincibleTimer = timeInvincible;

        if (animator != null)
            animator.SetTrigger("Hit");
    }
}

    void Launch()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Не призначено Projectile Prefab!");
            return;
        }

        Vector2 direction = lastMoveDirection;

        Vector2 spawnPosition = rb != null
            ? rb.position + direction * projectileSpawnDistance
            : (Vector2)transform.position +
              direction * projectileSpawnDistance;

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        Rigidbody2D projectileRb =
            projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
            projectileRb.linearVelocity = direction * projectileForce;
    }

    // Приховування попередньої репліки NPC
    void HideLastDialogueBubble()
    {
        if (lastNonPlayerCharacter != null)
        {
            if (lastNonPlayerCharacter.dialogueBubble != null)
                lastNonPlayerCharacter.dialogueBubble.SetActive(false);

            lastNonPlayerCharacter = null;
        }
    }

    // Показ репліки NPC
    void FindFriend()
    {
        if (TalkAction != null &&
            TalkAction.WasPressedThisFrame())
        {
            if (UIHandler.instance != null)
                UIHandler.instance.DisplayDialogue();
        }
    }

    void OnDisable()
    {
        if (TalkAction != null)
            TalkAction.Disable();
    }
}
