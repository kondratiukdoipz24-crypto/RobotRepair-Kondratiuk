
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance;

    public float displayTime = 4f;
    public UIDocument endScreenDocument;

    private VisualElement healthBar;
    private VisualElement m_NonPlayerDialogue;
    private VisualElement m_WinScreen;
    private VisualElement m_LoseScreen;

    private float currentHealthValue = 0f;
    private float m_TimerDisplay;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;

        UIDocument uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("Не знайдено основний UIDocument!");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;

        VisualElement container =
            root.Q<VisualElement>("HealthBarContainer");

        healthBar = root.Q<VisualElement>("HealthBar");

        if (container != null && healthBar != null)
        {
            container.RegisterCallback<GeometryChangedEvent>(
                evt => UpdateHealthBar()
            );

            UpdateHealthBar();
        }
        else
        {
            Debug.LogError("Не знайдено елементи здоров'я!");
        }

        m_NonPlayerDialogue =
            root.Q<VisualElement>("NPCDialogue");

        if (m_NonPlayerDialogue != null)
            m_NonPlayerDialogue.style.display = DisplayStyle.None;

        m_TimerDisplay = -1f;

        if (endScreenDocument == null)
        {
            Debug.LogError("Не призначено EndScreen UIDocument!");
            return;
        }

        VisualElement endRoot =
            endScreenDocument.rootVisualElement;

        m_LoseScreen =
            endRoot.Q<VisualElement>("LoseScreenContainer");

        m_WinScreen =
            endRoot.Q<VisualElement>("WinScreenContainer");

        if (m_LoseScreen != null)
            m_LoseScreen.style.display = DisplayStyle.None;

        if (m_WinScreen != null)
            m_WinScreen.style.display = DisplayStyle.None;
    }

    void Update()
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;

            if (m_TimerDisplay <= 0 &&
                m_NonPlayerDialogue != null)
            {
                m_NonPlayerDialogue.style.display =
                    DisplayStyle.None;
            }
        }
    }

    public void SetHealthValue(float value)
    {
        currentHealthValue = Mathf.Clamp01(value);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null)
            return;

        healthBar.style.width =
            Length.Percent(currentHealthValue * 100f);
    }

    public void DisplayDialogue()
    {
        if (m_NonPlayerDialogue == null)
            return;

        m_NonPlayerDialogue.style.display =
            DisplayStyle.Flex;

        m_TimerDisplay = displayTime;
    }

    public void DisplayWinScreen()
    {
        if (m_WinScreen == null)
        {
            Debug.LogError("Екран перемоги не знайдено!");
            return;
        }

        m_WinScreen.style.display = DisplayStyle.Flex;
        Debug.Log("Перемога!");
    }

    public void DisplayLoseScreen()
    {
        if (m_LoseScreen == null)
        {
            Debug.LogError("Екран поразки не знайдено!");
            return;
        }

        m_LoseScreen.style.display = DisplayStyle.Flex;
        Debug.Log("Поразка!");
    }
}
