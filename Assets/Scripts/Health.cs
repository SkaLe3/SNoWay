using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IInteractable
{
    [Header("Properties")]
    [SerializeField] private float CurrentHealth = 100f;
    [SerializeField] private float MaxHealth = 100f;
    [SerializeField] private bool bSnowHeals = false;

    public UnityEvent<float, float> OnHealthChanged;
    public UnityEvent OnDead;

    public bool IsDead
    {
        get { return CurrentHealth == 0f; }
    }

    public float HealthPercent
    {
        get { return CurrentHealth / MaxHealth; }
    }

    public void GetHit(GameObject hitObject)
    {
        float damage = 0f;
        FireProjectile fp = hitObject.GetComponent<FireProjectile>();
        if (fp != null)
        {
            if (!bSnowHeals) return;
            damage = fp.Damage;
        }

        SnowProjectile sp = hitObject.GetComponent<SnowProjectile>();
        if (sp != null)
        {
            damage = sp.SnowAmount;
            if (bSnowHeals) damage = -damage;

        }
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        float oldHealth = CurrentHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
        if (CurrentHealth == 0f)
        {
            OnDead?.Invoke();
        }
        OnHealthChanged?.Invoke(CurrentHealth, oldHealth - CurrentHealth);
    }

}
