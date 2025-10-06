using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SnowMan : MonoBehaviour, IInteractable
{
    [Header("Snowman Properties")]
    [SerializeField] private Healthbar Healthbar;


    public float TotalHealed => m_TotalHealed;
    private float m_TotalHealed = 0;

    private Health m_Health;
    private Animator m_Animator;

    private float m_DegradeTarget = 0f;
    private float m_DegradeValue = 0f;

    public void GetHit(GameObject hitObject)
    {

    }
    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Health = GetComponent<Health>();
        if (m_Health)
        {
            m_Health.OnHealthChanged.AddListener(HealthChangedHandle);
            m_Health.OnDead.AddListener(Died);
        }
    }

    void Start()
    {
        HealthChangedHandle(0, 0);
        m_DegradeValue = m_DegradeTarget;
        Healthbar.UpdateHealthbar(m_Health.HealthPercent);

    }

    void Update()
    {
        Healthbar.UpdateHealthbar(m_Health.HealthPercent);
        float interpSpeed = 1f;
        m_DegradeValue = Mathf.Lerp(m_DegradeValue, m_DegradeTarget, Mathf.Clamp01(interpSpeed * Time.deltaTime));
        m_DegradeValue = Mathf.Clamp(m_DegradeValue, 0.0f, 0.999f);
        m_Animator.SetFloat("Degrade", m_DegradeValue);
    }


    public void HealthChangedHandle(float currentHealth, float delta)
    {
        m_DegradeTarget = (1 - Mathf.Clamp01(m_Health.HealthPercent));
        if (delta < 0)
            m_TotalHealed -= delta;
    }

    public void Died()
    {
        GameManager.Instance.GameOver();
    }


}
