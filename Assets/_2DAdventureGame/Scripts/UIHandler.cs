using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance;

    public float displayTime = 4f;
    public UIDocument endScreenDocument;

    private VisualElement healthBar;
    private VisualElement m_NonPlayerDialogue;

    private VisualElement m_WinContainer;
    private VisualElement m_LoseContainer;

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
        if (uiDocument != null)
        {
            VisualElement root = uiDocument.rootVisualElement;
            VisualElement container = root.Q<VisualElement>("HealthBarContainer");
            healthBar = root.Q<VisualElement>("HealthBar");

            if (container != null && healthBar != null)
            {
                container.RegisterCallback<GeometryChangedEvent>(evt => UpdateHealthBar());
                UpdateHealthBar();
            }

            m_NonPlayerDialogue = root.Q<VisualElement>("NPCDialogue");
            if (m_NonPlayerDialogue != null)
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
        }

        m_TimerDisplay = -1f;

        if (endScreenDocument == null)
        {
            Debug.LogError("Не призначено EndScreen UIDocument!");
            return;
        }

        VisualElement endRoot = endScreenDocument.rootVisualElement;

        m_LoseContainer = endRoot.Q<VisualElement>("LoseScreenContainer");
        m_WinContainer = endRoot.Q<VisualElement>("WinScreenContainer");

        // На старті ховаємо обидва екрани і ставимо прозорість 0
        if (m_LoseContainer != null)
        {
            m_LoseContainer.style.display = DisplayStyle.None;
            m_LoseContainer.style.opacity = 0f;
        }

        if (m_WinContainer != null)
        {
            m_WinContainer.style.display = DisplayStyle.None;
            m_WinContainer.style.opacity = 0f;
        }
    }

    void Update()
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;

            if (m_TimerDisplay <= 0 && m_NonPlayerDialogue != null)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
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

        healthBar.style.width = Length.Percent(currentHealthValue * 100f);
    }

    public void DisplayDialogue()
    {
        if (m_NonPlayerDialogue == null)
            return;

        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_TimerDisplay = displayTime;
    }

    public void DisplayWinScreen()
    {
        if (m_LoseContainer != null)
        {
            m_LoseContainer.style.display = DisplayStyle.None;
            m_LoseContainer.style.opacity = 0f;
        }

        if (m_WinContainer != null)
        {
            m_WinContainer.style.display = DisplayStyle.Flex;
            m_WinContainer.style.opacity = 1f; // Робимо картинку перемоги видимою!
        }

        Debug.Log("Перемога!");
    }

    public void DisplayLoseScreen()
    {
        if (m_WinContainer != null)
        {
            m_WinContainer.style.display = DisplayStyle.None;
            m_WinContainer.style.opacity = 0f;
        }

        if (m_LoseContainer != null)
        {
            m_LoseContainer.style.display = DisplayStyle.Flex;
            m_LoseContainer.style.opacity = 1f; // Робимо картинку поразки видимою!
        }

        Debug.Log("Поразка!");
    }
}