using UnityEngine;

public class HealthCollectionManager : MonoBehaviour
{
    public static HealthCollectionManager Instance;

    public int totalItems;
    public int collectedItems = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Автоматично рахує всі предмети їжі, які лежать всередині цього об'єкта
        totalItems = transform.childCount;
    }

    public void CollectItem()
    {
        collectedItems++;
        Debug.Log($"Зібрано: {collectedItems} з {totalItems}");
    }

    public bool AreAllCollected()
    {
        return collectedItems >= totalItems;
    }
}