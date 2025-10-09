using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BallMovement : MonoBehaviour
{
    [Header("References")]
    public Transform CursorTarget;
    [SerializeField] private AudioSource RollingSound;

    [Header("Movement")]
    [SerializeField] private float BaseAcceleration = 20f;
    [SerializeField] private float BaseMaxSpeed = 20f;
    [SerializeField] private float MaxSpeedExponent = 0.2f;
    [SerializeField] private float AccelerationExponent = 0.2f;
    [SerializeField] private float StopDistance = 0.1f;

    private float m_BaseRadius = 1f;


    private Rigidbody m_Rb;
    private BallGrowing m_Bg;


    private float m_CurrentMaxSpeed;
    private float m_CurrentAcceleration;

    void Awake()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_CurrentMaxSpeed = BaseMaxSpeed;
        m_CurrentAcceleration = BaseAcceleration;
    }

    void Start()
    {
        m_Bg = GetComponent<BallGrowing>();
        if (m_Bg)
        {
            m_BaseRadius = m_Bg.StartRadius;
        }
    }

    void FixedUpdate()
    {
        if (m_Bg)
        {
            StopDistance = m_Bg.Radius;
        }
        UpdateMovementParameters();
        if (!Mouse.current.rightButton.isPressed)
        {
            AddMovement();
        }
    }

    void Update()
    {
        UpdateRollingSound();
    }
    void UpdateRollingSound()
    {
        float velocity = m_Rb.linearVelocity.magnitude;
        float factorVolume = Mathf.Clamp01(velocity / 8f);
        float factorPitch = Mathf.Clamp01(velocity / 15f);
        RollingSound.pitch = Mathf.Lerp(0.8f, 1.2f, factorPitch);
        RollingSound.volume = Mathf.Lerp(0f, 1f, factorVolume);
    }

    void UpdateMovementParameters()
    {
        if (!m_Bg) return;

        float radius = m_Bg.Radius;
        float sizeFactor = Mathf.Max(0.01f, radius / m_BaseRadius);

        m_CurrentMaxSpeed = BaseMaxSpeed * Mathf.Pow(sizeFactor, MaxSpeedExponent);
        m_CurrentAcceleration = BaseAcceleration * Mathf.Pow(sizeFactor, AccelerationExponent);
    }

    void AddMovement()
    {
        if (CursorTarget == null) return;


        Vector3 direction = CursorTarget.position - m_Rb.position;
        direction.y = 0f;

        float distanceSqr = direction.sqrMagnitude;
        if (distanceSqr > StopDistance * StopDistance)
        {
            m_Rb.AddForce(direction.normalized * m_CurrentAcceleration, ForceMode.Acceleration);

            float currentSpeedSqr = m_Rb.linearVelocity.sqrMagnitude;
            float maxSpeedSqr = m_CurrentMaxSpeed * m_CurrentMaxSpeed;
            if (currentSpeedSqr > maxSpeedSqr)
            {
                Vector3 clampedVelocity = m_Rb.linearVelocity.normalized * m_CurrentMaxSpeed;
                Vector3 counterForce = (clampedVelocity - m_Rb.linearVelocity) / Time.fixedDeltaTime;
                m_Rb.AddForce(counterForce, ForceMode.Acceleration);
            }
        }
    }
}
