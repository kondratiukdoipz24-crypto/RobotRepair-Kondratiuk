
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public RobotController player;
    public UIHandler uiHandler;

    private EnemyController[] enemies;
    private bool gameEnded = false;

    void Start()
    {
        enemies = FindObjectsByType<EnemyController>(
            FindObjectsSortMode.None
        );

        Debug.Log("Знайдено ворогів: " + enemies.Length);
    }

    void Update()
    {
        if (gameEnded)
            return;

        // Перевірка поразки
        if (player != null && player.health <= 0)
        {
            gameEnded = true;

            Debug.Log("Здоров'я робота дорівнює 0!");

            uiHandler.DisplayLoseScreen();

            Invoke(nameof(ReloadScene), 3f);
            return;
        }

        // Перевірка перемоги
        if (enemies.Length > 0 && AllEnemiesFixed())
        {
            gameEnded = true;

            Debug.Log("Усі вороги полагоджені!");

            uiHandler.DisplayWinScreen();

            Invoke(nameof(ReloadScene), 3f);
        }
    }

    bool AllEnemiesFixed()
    {
        foreach (EnemyController enemy in enemies)
        {
            if (!enemy.isBroken)
                return false;
        }

        return true;
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }
}
