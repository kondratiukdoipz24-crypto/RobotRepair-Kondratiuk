using UnityEngine;
using UnityEngine.InputSystem;

public class RobotController : MonoBehaviour
{
    [Header("Рух")]
    public float speed = 4f;
    public float jumpForce = 11f;

    [Header("Здоров'я")]
    public int maxHealth = 5;
    public int startingHealth = 1;
    private int currentHealth;
    private int healthGainedTotal = 0;

    public int health => currentHealth;
    public int HealthGainedTotal => healthGainedTotal;

    public float timeInvincible = 2f;
    private bool isInvincible;
    private float invincibleTimer;

    [Header("Бій")]
    public GameObject projectilePrefab;
    public float projectileForce = 10f;
    public float projectileSpawnDistance = 0.8f;

    [Header("Діалоги")]
    public InputAction TalkAction;

    private NonPlayerCharacter lastNonPlayerCharacter;
    private Rigidbody2D rb;
    private Animator animator;

    private float moveX;
    private Vector2 lastMoveDirection = Vector2.right;
    private bool gameEnded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = Mathf.Clamp(startingHealth, 0, maxHealth);
        healthGainedTotal = 0;

        if (UIHandler.instance != null)
            UIHandler.instance.SetHealthValue((float)currentHealth / maxHealth);

        if (TalkAction != null)
            TalkAction.Enable();
    }

    void Update()
    {
        if (gameEnded)
            return;

        moveX = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
                moveX = -1f;

            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
                moveX = 1f;

            if (moveX != 0f)
                lastMoveDirection = new Vector2(moveX, 0f);

            bool isGrounded = rb != null && Mathf.Abs(rb.linearVelocity.y) < 0.15f;

            if ((Keyboard.current.wKey.wasPressedThisFrame ||
                 Keyboard.current.upArrowKey.wasPressedThisFrame ||
                 Keyboard.current.leftShiftKey.wasPressedThisFrame)
                 && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                Launch();
        }

        if (animator != null)
        {
            animator.SetFloat("Look X", lastMoveDirection.x);
            animator.SetFloat("Look Y", 0f);
            animator.SetFloat("Speed", Mathf.Abs(moveX));
        }

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
                isInvincible = false;
        }

        CheckNPC();
    }

    void FixedUpdate()
    {
        if (rb == null || gameEnded)
            return;

        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);
    }

    public void ChangeHealth(int amount)
    {
        if (gameEnded)
            return;

        if (amount < 0 && isInvincible)
            return;

        if (amount > 0)
            healthGainedTotal += amount;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (UIHandler.instance != null)
            UIHandler.instance.SetHealthValue((float)currentHealth / maxHealth);

        if (amount < 0)
        {
            isInvincible = true;
            invincibleTimer = timeInvincible;

            if (animator != null)
                animator.SetTrigger("Hit");
        }
    }

   /* public void TriggerEndGame(bool isWin)
    {
        if (gameEnded)
            return;

        gameEnded = true;
        moveX = 0f;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetFloat("Speed", 0f);

        if (UIHandler.instance != null)
        {
            if (isWin)
                UIHandler.instance.DisplayWinScreen();
            else
                UIHandler.instance.DisplayLoseScreen();
        }

        Time.timeScale = 0f;
    }
*/
public void TriggerEndGame(bool isWin)
    {
        if (gameEnded)
            return;

        gameEnded = true;
        moveX = 0f;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetFloat("Speed", 0f);

        if (UIHandler.instance != null)
        {
            if (isWin)
                UIHandler.instance.DisplayWinScreen();
            else
                UIHandler.instance.DisplayLoseScreen();
        }

        // Time.timeScale = 0f;  <-- Ось цей рядок блокував малювання екрану!
    }
    void Launch()
    {
        if (projectilePrefab == null || rb == null)
            return;

        Vector2 direction = lastMoveDirection;
        Vector2 spawnPosition = rb.position + direction * projectileSpawnDistance;

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
            projectileRb.linearVelocity = direction * projectileForce;
    }

    void CheckNPC()
    {
        if (rb == null)
            return;

        RaycastHit2D hit = Physics2D.Raycast(
            rb.position + Vector2.up * 0.2f,
            lastMoveDirection,
            1.5f,
            LayerMask.GetMask("NPC")
        );

        if (hit.collider != null)
        {
            NonPlayerCharacter npc = hit.collider.GetComponent<NonPlayerCharacter>();

            if (npc != null)
            {
                if (lastNonPlayerCharacter != null &&
                    lastNonPlayerCharacter != npc &&
                    lastNonPlayerCharacter.dialogueBubble != null)
                {
                    lastNonPlayerCharacter.dialogueBubble.SetActive(false);
                }

                if (npc.dialogueBubble != null)
                    npc.dialogueBubble.SetActive(true);

                lastNonPlayerCharacter = npc;
                FindFriend();
                return;
            }
        }

        HideLastDialogueBubble();
    }

    void FindFriend()
    {
        if (TalkAction != null && TalkAction.WasPressedThisFrame())
        {
            if (UIHandler.instance != null)
                UIHandler.instance.DisplayDialogue();
        }
    }

    void HideLastDialogueBubble()
    {
        if (lastNonPlayerCharacter != null)
        {
            if (lastNonPlayerCharacter.dialogueBubble != null)
                lastNonPlayerCharacter.dialogueBubble.SetActive(false);

            lastNonPlayerCharacter = null;
        }
    }

    void OnDisable()
    {
        if (TalkAction != null)
            TalkAction.Disable();
    }
}