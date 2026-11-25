
using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    private bool broken = false;

    public bool isBroken
    {
        get { return broken; }
    }

    [Header("Movement")]
    public float speed = 1f;
    public float changeTime = 3f;
    public float pauseTime = 2f;
    public bool vertical = false;

    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D[] enemyColliders;
    private SpriteRenderer[] spriteRenderers;
    private Camera mainCamera;

    private float timer;
    private Vector2 direction = Vector2.right;
    private bool isPaused;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyColliders = GetComponentsInChildren<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        mainCamera = Camera.main;

        currentHealth = maxHealth;
        timer = changeTime;

        if (maxHealth < 1)
        {
            maxHealth = 3;
            currentHealth = maxHealth;
        }

        SetRandomDirection();

        Debug.Log("Ворог: " + name +
                  ". Здоров'я: " + currentHealth + "/" + maxHealth);
    }

    void Update()
    {
        if (isPaused || broken)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SetRandomDirection();
            timer = changeTime;
        }

        if (animator != null)
        {
            animator.SetFloat("Move X", direction.x);
            animator.SetFloat("Move Y", direction.y);
        }
    }

    void SetRandomDirection()
    {
        if (vertical)
        {
            direction = Random.value < 0.5f
                ? Vector2.up
                : Vector2.down;
        }
        else
        {
            direction = Random.value < 0.5f
                ? Vector2.left
                : Vector2.right;
        }
    }

    void FixedUpdate()
    {
        if (isPaused || broken || rb == null)
            return;

        Vector2 position = rb.position;

        Vector2 nextPosition =
            position + direction * speed * Time.fixedDeltaTime;

        if (mainCamera != null)
        {
            float distance = -mainCamera.transform.position.z;

            Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(
                new Vector3(0, 0, distance)
            );

            Vector3 topRight = mainCamera.ViewportToWorldPoint(
                new Vector3(1, 1, distance)
            );

            float halfWidth = enemyColliders.Length > 0
                ? enemyColliders[0].bounds.extents.x
                : 0.2f;

            float halfHeight = enemyColliders.Length > 0
                ? enemyColliders[0].bounds.extents.y
                : 0.2f;

            float minX = bottomLeft.x + halfWidth;
            float maxX = topRight.x - halfWidth;
            float minY = bottomLeft.y + halfHeight;
            float maxY = topRight.y - halfHeight;

            if (nextPosition.x < minX || nextPosition.x > maxX)
            {
                direction.x *= -1;
                nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);
                timer = changeTime;
            }

            if (nextPosition.y < minY || nextPosition.y > maxY)
            {
                direction.y *= -1;
                nextPosition.y = Mathf.Clamp(nextPosition.y, minY, maxY);
                timer = changeTime;
            }
        }

        rb.MovePosition(nextPosition);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (broken)
            return;

        RobotController player =
            other.GetComponentInParent<RobotController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (broken || currentHealth <= 0 || damage <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        Debug.Log(
            name + ": здоров'я " +
            currentHealth + "/" + maxHealth
        );

        if (currentHealth <= 0)
        {
            RepairEnemy();
            return;
        }

        if (!isPaused)
        {
            StartCoroutine(PauseMovement());
        }
    }

    void RepairEnemy()
    {
        broken = true;
        isPaused = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }

        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            sprite.enabled = false;
        }

        foreach (Collider2D enemyCollider in enemyColliders)
        {
            enemyCollider.enabled = false;
        }

        Debug.Log("Ворога полагоджено: " + name);
    }

    IEnumerator PauseMovement()
    {
        isPaused = true;

        yield return new WaitForSeconds(pauseTime);

        if (!broken)
        {
            isPaused = false;
            timer = changeTime;
        }
    }
}
