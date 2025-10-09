using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyShooter : MonoBehaviour
{
    [Header("Fireball Settings")]
    [SerializeField] private GameObject FireballPrefab;
    [SerializeField] private float CooldownMin = 1f;
    [SerializeField] private float CooldownMax = 3f;

    [Header("Shot Settings")]
    [SerializeField] float Gravity = 9.81f;
    [SerializeField] float MinAngle = 25f;
    [SerializeField] float MaxAngle = 45f;

    private float m_Cooldown;
    private GameObject m_Target;


    void Start()
    {
        m_Cooldown = Random.Range(CooldownMin, CooldownMax);
    }

    public void StartAttacking(GameObject target)
    {
        m_Target = target;
        InvokeRepeating("Shoot", 0.5f, m_Cooldown);
    }

    void StopAttacking()
    {
        CancelInvoke("Shoot");
    }

    void Shoot()
    {
        if (m_Target == null) return;
        Vector3 shotSource = transform.position + new Vector3(0f, 2f, 0f);
        Vector3 shotTarget = m_Target.transform.position;
        Vector3 launchVelocity = CalculaterInitialProjectileVelocity(shotSource, shotTarget);

        GameObject projectile = FireProjectile(shotSource, launchVelocity);
        FireProjectile fireProjectile = projectile.GetComponent<FireProjectile>();
        fireProjectile.Gravity = Gravity;

        // Play shot sound here;
    }

    Vector3 CalculaterInitialProjectileVelocity(Vector3 shotSource, Vector3 shotTarget)
    {
        Vector3 diff = shotTarget - shotSource;
        Vector3 diffXZ = new Vector3(diff.x, 0, diff.z);
        float distance = diffXZ.magnitude;
        float yOffset = diff.y;
        float t = Mathf.InverseLerp(25f, 45f, distance);
        float angle = Mathf.Lerp(MaxAngle, MinAngle, t);
        float radAngle = angle * Mathf.Deg2Rad;
        float velocity = Mathf.Sqrt((Gravity * distance * distance) / (2f * Mathf.Cos(radAngle) * Mathf.Cos(radAngle) * distance * Mathf.Tan(radAngle) - yOffset));
        Vector3 velocityDir = diffXZ.normalized;
        Vector3 launchVelocity = velocityDir * velocity * Mathf.Cos(radAngle) + Vector3.up * velocity * Mathf.Sin(radAngle);
        return launchVelocity;
    }

    GameObject FireProjectile(Vector3 shotSource, Vector3 launchVelocity)
    {
        GameObject fireball = Instantiate(FireballPrefab, shotSource, Quaternion.identity);
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        rb.linearVelocity = launchVelocity;

        Collider shooterCollider = GetComponent<Collider>();
        Collider fireballCollider = fireball.GetComponent<Collider>();

        if (shooterCollider && fireballCollider)
        {
            Physics.IgnoreCollision(fireballCollider, shooterCollider);
        }
        return fireball;
    }
}
