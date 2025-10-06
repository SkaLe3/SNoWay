using UnityEngine;

[RequireComponent(typeof(BallDistanceTracker))]
public class BallGrowing : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private ChangeParticleSize SnowPath;
    [SerializeField] private ChangeSnowProperties SnowField;

    [Header("Properties")]
    [SerializeField] private float GrowthRate = 1f;
    [SerializeField] public float StartRadius = 1f;
    [SerializeField] private float MaxRadius = 25f;
    private BallDistanceTracker m_Tracker;

    public float Radius => m_Radius;

    private float m_Radius;

    [SerializeField]private float m_SnowStorage = 0f;
    private float m_SnowMax = 0f;
    [SerializeField]private float m_SnowReserved = 0f;

    private AudioSource m_EffectAudioSource;



    public void PlayThrowSound()
    {
        if (m_EffectAudioSource != null && m_EffectAudioSource.clip != null)
        {
            m_EffectAudioSource.Stop();
            m_EffectAudioSource.Play();
        }
    }
    public bool CanShoot(float shotCost)
    {
        return (m_SnowStorage - m_SnowReserved) > shotCost;
    }
    public void UseSnow(float snowAmount)
    {
        m_SnowReserved += snowAmount;
    }

    void Awake()
    {
        m_Tracker = GetComponent<BallDistanceTracker>();
        transform.localScale = Vector3.one * StartRadius * 2;
        m_EffectAudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (SnowPath)
        {
            SnowPath.StartSize = GetTrailSize();
        }
    }


    void Update()
    {
        UpdateSnowAmount();
        GrowSize();
        UseReservedSnow();
        UpdateSnowPathWidth();
        UpdateSnowHeight();
    }

    void UpdateSnowAmount()
    {
        if (m_Radius != MaxRadius)
        {
            m_SnowStorage += m_Tracker.LastDistanceDelta;
        }
        if (m_SnowStorage > m_SnowMax)
        {
            m_SnowMax = m_SnowStorage;
        }
    }

    void GrowSize()
    {
        m_Radius = Mathf.Pow(StartRadius * StartRadius * StartRadius + GrowthRate * m_SnowStorage, 1f / 3f);
        m_Radius = Mathf.Clamp(m_Radius, StartRadius, MaxRadius);
        transform.localScale = Vector3.one * m_Radius * 2;
    }

    void UseReservedSnow()
    {
        float useAmount = m_SnowReserved * Time.deltaTime;
        if (m_SnowReserved < useAmount || m_SnowReserved < 0.1f)
        {
            useAmount = m_SnowReserved;
        }
        m_SnowStorage = Mathf.Clamp(m_SnowStorage - useAmount, 0, m_SnowStorage);
        m_SnowReserved -= useAmount;
    }

    void UpdateSnowPathWidth()
    {
        if (SnowPath)
        {
            SnowPath.NewSize = GetTrailSize();
        }
    }

    void UpdateSnowHeight()
    {
        if (SnowField)
        {
            SnowField.NewHeight = SnowField.StartHeight + ((m_Radius - StartRadius) / MaxRadius) * SnowField.StartHeight * 0.7f;
        }
    }

    float GetTrailSize()
    {
        return 4.0f * Mathf.Pow(m_Radius, 0.75f);
    }
}
