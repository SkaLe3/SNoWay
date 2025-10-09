using UnityEngine;

public class SnowProjectile : MonoBehaviour
{

    [Header("Snowball Settings")]
    [SerializeField] private GameObject HitSoundPrefab;
    [SerializeField] private GameObject HitParticlesPrefab;
    [SerializeField] private float SizeFactor = 1f;
    public float SnowAmount => m_SnowAmount;
    private float m_SnowAmount;

    public float Gravity = 9.81f;

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

    public void SetSize(float snowAmount)
    {
        m_SnowAmount = snowAmount;
        double baseValue = (3 * m_SnowAmount) / (4 * Mathf.PI);
        float radius = Mathf.Pow((float)baseValue, (float)(1.0 / 3.0));
        radius *= 0.6f;
        radius *= SizeFactor;
        transform.localScale = new Vector3(radius, radius, radius);
    }


    void OnCollisionEnter(Collision collision)
    {
        SnowProjectile other = collision.gameObject.GetComponent<SnowProjectile>();
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
            soundObject.GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
            soundObject.GetComponent<AudioSource>().volume = Random.Range(0.8f, 1.2f);
            Destroy(soundObject, 1f);
        }
        if (HitParticlesPrefab != null)
        {
            GameObject particlesObject = Instantiate(HitParticlesPrefab, impactPoint, Quaternion.identity);
            Destroy(particlesObject, 2f);
        }

        Destroy(gameObject, 0.05f); 
            
    }
}
