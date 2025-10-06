using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnowShooter : MonoBehaviour
{
    [Header("Snowball Settings")]
    [SerializeField] private GameObject SnowballPrefab;
    [SerializeField] private float Cooldown = 0.3f;
    [SerializeField] private float ShotCost = 10f;

    [Header("Shot Settings")]
    [SerializeField] float Gravity = 9.81f;
    [SerializeField] float MinAngle = 25f;
    [SerializeField] float MaxAngle = 45f;

    public float TotalBallsThrowed => m_TotalBallsThrowed;
    private float m_TotalBallsThrowed = 0;

    private float m_LastShootTime;

    private PlayerController m_PlayerController;

    private InputAction m_ShootAction;

    void OnEnable()
    {
        m_ShootAction = new InputAction(binding: "<Mouse>/leftButton");
        m_ShootAction.Enable();
    }
    void OnDisable()
    {
        m_ShootAction.Disable();
    }

    void Awake()
    {
        m_PlayerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Wall"), LayerMask.NameToLayer("Projectile"), true);
    }

    void Update()
    {
        if (!m_ShootAction.WasPerformedThisFrame()) return;
        if (!m_PlayerController) return;

        GameObject shooterObject = m_PlayerController.ShooterObject;
        BallGrowing bg = shooterObject.GetComponent<BallGrowing>();

        if (Time.time - m_LastShootTime < Cooldown)
            return;
            
        m_LastShootTime = Time.time;
        if (bg != null && bg.CanShoot(ShotCost))
        {
            ShootProjectile();
            bg.UseSnow(ShotCost);
            bg.PlayThrowSound();
            m_TotalBallsThrowed++;
        }

    }

    void ShootProjectile()
    {
        GameObject shooterObject = m_PlayerController.ShooterObject;
        Vector3 shotSource = CalculateShotSource(shooterObject);
        Vector3 shotTarget = m_PlayerController.ShootTarget;
        Vector3 launchVelocity = CalculaterInitialProjectileVelocity(shotSource, shotTarget);

        GameObject projectile = FireProjectile(shooterObject, shotSource, launchVelocity);
        SnowProjectile snowProjectile = projectile.GetComponent<SnowProjectile>();
        snowProjectile.SetSize(ShotCost);
        snowProjectile.Gravity = Gravity;
    }

    Vector3 CalculateShotSource(GameObject shooterObject)
    {
        Vector3 shotSource = shooterObject.transform.position;
        Vector3 shotTarget = m_PlayerController.ShootTarget;
        shotSource.y = 0;
        shotTarget.y = 0;
        Vector3 shotVector = shotTarget - shotSource;
        Vector3 shotDirection = shotVector.normalized;
        Vector3 rotationAxis = Vector3.Cross(shotDirection, Vector3.up);
        Quaternion pitchRotation = Quaternion.AngleAxis(60f, rotationAxis);
        Vector3 shotDirectionPitched = pitchRotation * shotDirection;
        Vector3 shotMuzzle = shotSource + shotDirectionPitched * shooterObject.GetComponent<BallGrowing>().Radius;
        return shotMuzzle;
    }

    Vector3 CalculaterInitialProjectileVelocity(Vector3 shotSource, Vector3 shotTarget)
    {
        Vector3 diff = shotTarget - shotSource;
        Vector3 diffXZ = new Vector3(diff.x, 0, diff.z);
        float distance = diffXZ.magnitude;
        float yOffset = diff.y;
        float t = Mathf.InverseLerp(15f, 40f, distance);
        float angle = Mathf.Lerp(MaxAngle, MinAngle, t);
        float radAngle = angle * Mathf.Deg2Rad;
        float velocity = Mathf.Sqrt((Gravity * distance * distance) / (2f * Mathf.Cos(radAngle) * Mathf.Cos(radAngle) * distance * Mathf.Tan(radAngle) - yOffset));
        Vector3 velocityDir = diffXZ.normalized;
        Vector3 launchVelocity = velocityDir * velocity * Mathf.Cos(radAngle) + Vector3.up * velocity * Mathf.Sin(radAngle);
        return launchVelocity;
    }

    GameObject FireProjectile(GameObject shooterObject, Vector3 shotSource, Vector3 launchVelocity)
    {
        GameObject snowball = Instantiate(SnowballPrefab, shotSource, Quaternion.Euler(Vector3.zero));
        Rigidbody rb = snowball.GetComponent<Rigidbody>();
        rb.linearVelocity = launchVelocity;

        // Ignore snowball
        Collider shooterCollider = shooterObject.GetComponent<Collider>();
        Collider snowballCollider = snowball.GetComponent<Collider>();
        if (shooterCollider && snowballCollider)
        {
            Physics.IgnoreCollision(snowballCollider, shooterCollider);
        }
        return snowball;
    }
}
