
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 1f;
    public bool vertical;
    public float changeTime = 3f;

    public int maxHealth = 3;
    private int currentHealth;

    Rigidbody2D rigidbody2d;
    Animator animator;

    float timer;
    int direction = 1;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        timer = changeTime;
        currentHealth = maxHealth;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            direction *= -1;
            timer = changeTime;
        }

        if (vertical)
        {
            animator.SetFloat("Move X", 0f);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0f);
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;

        if (vertical)
            position.y += speed * direction * Time.fixedDeltaTime;
        else
            position.x += speed * direction * Time.fixedDeltaTime;

        rigidbody2d.MovePosition(position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        RobotController player = other.GetComponent<RobotController>();

        if (player != null)
            player.ChangeHealth(-1);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Здоров'я ворога: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0)
            Destroy(gameObject);
    }
}
