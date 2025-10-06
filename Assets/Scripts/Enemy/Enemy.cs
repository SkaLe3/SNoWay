using NUnit.Framework.Interfaces;
using UnityEngine;

public class Enemy : MonoBehaviour, IInteractable
{
    public GameObject AttackTarget;

    [Header("Properties")]
    [SerializeField] private float MinSpeed = 4f;
    [SerializeField] private float MaxSpeed = 5f;
    [SerializeField] private Healthbar m_Healthbar;
    [SerializeField] private AudioClip m_DamageClip;
    [SerializeField] private AudioClip m_DeathClip;


    private float m_AttackDistance;

    private float m_MoveSpeed; 

    private Health m_Health;
    private AudioSource m_AudioSource;
    private EnemyShooter m_EnemyShooter;


    public enum EnemyState { MovingToTarget, Attacking }
    private EnemyState m_EnemyState = EnemyState.MovingToTarget;

    public void GetHit(GameObject hitObject)
    {

    }

    public void HealthChangeHandle(float currentHealth, float delta)
    {
        if (!m_Health.IsDead)
        {
            if (m_DamageClip != null)
            {
                m_AudioSource.clip = m_DamageClip;
                m_AudioSource.Play();
            }
        }
    }

    public void Died()
    {
        if (m_DeathClip != null)
        {
            m_AudioSource.clip = m_DeathClip;
            m_AudioSource.Play();
        }
        GameManager.Instance.EnemyDefeated();
        Destroy(gameObject, 0.5f);
    }

    void Awake()
    {
        m_Health = GetComponent<Health>();
        m_AudioSource = GetComponent<AudioSource>();
        m_EnemyShooter = GetComponent<EnemyShooter>();
    }

    void Start()
    {
        m_AttackDistance = Random.Range(20f, 40f);
        m_MoveSpeed = Random.Range(MinSpeed, MaxSpeed);
        m_Healthbar.UpdateHealthbar(m_Health.HealthPercent);
        m_Health.OnHealthChanged.AddListener(HealthChangeHandle);
        m_Health.OnDead.AddListener(Died);
        
    }


    void Update()
    {
        m_Healthbar.UpdateHealthbar(m_Health.HealthPercent);

        if (AttackTarget == null) return;

        Vector3 direction = AttackTarget.transform.position - transform.position;
        direction.y = 0;
        if (direction.magnitude < m_AttackDistance)
        {
            if (m_EnemyState != EnemyState.Attacking)
            {
                if (m_EnemyShooter)
                {
                    m_EnemyShooter.StartAttacking(AttackTarget);
                    Debug.Log("StartAttaking");
                }
            }
            m_EnemyState = EnemyState.Attacking;
        }

        if (m_EnemyState == EnemyState.MovingToTarget)
        {
            transform.position += direction.normalized * m_MoveSpeed * Time.deltaTime;
        }
    }
}
