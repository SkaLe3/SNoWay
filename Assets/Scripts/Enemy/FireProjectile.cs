using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [Header("Fireball Settings")]
    [SerializeField] private GameObject HitSoundPrefab;
    [SerializeField] private GameObject HitParticlePrefab;

    public float Damage;
    public float Gravity;

    private Rigidbody m_Rb;

    void Start()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_Rb.useGravity = false;
    }

    void FixedUpdate()
    {
        Vector3 customGravity = Vector3.down * Gravity;
        m_Rb.AddForce(customGravity, ForceMode.Acceleration);

       
    }

    void OnCollisionEnter(Collision collision)
    {
        FireProjectile other = collision.gameObject.GetComponent<FireProjectile>();
        if (other) return;

        IInteractable[] interactables = collision.gameObject.GetComponents<IInteractable>();
        foreach (var i in interactables)
        {
            i.GetHit(gameObject);
        }

        ContactPoint contact = collision.contacts[0];
        Vector3 impactPoint = contact.point;
        if (HitSoundPrefab != null)
        {
            GameObject soundObject = Instantiate(HitSoundPrefab, impactPoint, Quaternion.identity);
            Destroy(soundObject, 1f);
        }
        if (HitParticlePrefab != null)
        {
            GameObject particlesObject = Instantiate(HitParticlePrefab, impactPoint, Quaternion.identity);
            Destroy(particlesObject, 2f);
        }

        Destroy(gameObject, 0.05f); 
    }
}
