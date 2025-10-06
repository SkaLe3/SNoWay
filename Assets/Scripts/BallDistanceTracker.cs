using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class BallDistanceTracker : MonoBehaviour
{

    public float TotalDistance => m_TotalDistance;
    public float LastDistanceDelta => m_LastDistanceDelta;

    private Rigidbody m_Rb;
    private SphereCollider m_Sphere;
    private float m_TotalDistance = 0f;
    private float m_LastDistanceDelta = 0f;

    void Awake()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_Sphere = GetComponent<SphereCollider>();
    }


    void FixedUpdate()
    {
        float angularSpeed = m_Rb.angularVelocity.magnitude;
        float radius = m_Sphere.radius * transform.localScale.x;
        float linearSpeed = angularSpeed * radius;
        m_LastDistanceDelta = linearSpeed * Time.fixedDeltaTime;
        m_TotalDistance += m_LastDistanceDelta;
    }
}
