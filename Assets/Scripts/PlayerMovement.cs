using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float CameraMoveSpeed = 5f;
    [SerializeField] private float CameraAcceleration = 10f;
    [SerializeField] private float CameraDeceleration = 15f;

    [Header("Rotation Settings")]
    [SerializeField] private float CameraRotationSpeed = 120f;
    [SerializeField] private float CameraRotationAcceleration = 300f;
    [SerializeField] private float CameraRotationDeceleration = 500f;

    private Vector3 m_InputVector;
    private Vector3 m_LastInputVector;
    private Vector3 m_CurrentVelocity;

    private Vector3 m_RotationInput;
    private Vector3 m_CurrentRotationVelocity;

    public void AddMovementInput(Vector3 direction)
    {
        m_InputVector += direction;
    }

    public void AddRotationInput(Vector3 deltaRotation)
    {
        m_RotationInput += deltaRotation;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        ConsumeInputVector();
        ConsumeRotationVector();

    }

    void HandleMovement()
    {
        Vector3 inputDir = m_InputVector.normalized;
        Vector3 targetVelocity = inputDir * CameraMoveSpeed;
        float lerpRate = (targetVelocity.sqrMagnitude > 0.01f) ? CameraAcceleration : CameraDeceleration;
        m_CurrentVelocity = Vector3.MoveTowards(m_CurrentVelocity, targetVelocity, lerpRate * Time.deltaTime);

        transform.position += m_CurrentVelocity * Time.deltaTime;
    }

    void HandleRotation()
    {
        Vector3 targetRotationVel = m_RotationInput * CameraRotationSpeed;
        float lerpRate = (targetRotationVel.sqrMagnitude > 0.01f) ? CameraRotationAcceleration : CameraRotationDeceleration;
        m_CurrentRotationVelocity = Vector3.MoveTowards(m_CurrentRotationVelocity, targetRotationVel, lerpRate * Time.deltaTime);
        transform.Rotate(m_CurrentRotationVelocity * Time.deltaTime, Space.Self);
    }

    void ConsumeInputVector()
    {
        m_LastInputVector = m_InputVector;
        m_InputVector = Vector3.zero;
    }

    void ConsumeRotationVector()
    {
        m_RotationInput = Vector3.zero;
    }
}
