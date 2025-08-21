using UnityEngine;
using System;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField, Min(1)] private float maxHealth = 100f;
    [SerializeField, Min(0)] private float startHealth = 100f;
    [SerializeField, Tooltip("Минимальная задержка между получением урона (сек)")]
    private float damageCooldown = 0.1f;
    [SerializeField] private bool destroyOnDeath = false;

    [Header("Runtime Info")]
    [SerializeField] private float currentHealth;
    [SerializeField] private bool isDead;

    public float MaxHealth => maxHealth;
    public float Current => currentHealth;
    public bool IsDead => isDead;

    public event Action<float, float> OnDamaged;
    public event Action<float, float> OnHealed;
    public event Action OnDied;
    public event Action<float, float> OnChanged;

    private float lastDamageTime;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(startHealth, 0, maxHealth);
        lastDamageTime = -999f;
        isDead = currentHealth <= 0;
        NotifyChanged();
    }

    public void Heal(float amount)
    {
        if (amount <= 0 || isDead) return;

        float old = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealed?.Invoke(currentHealth - old, currentHealth);
        isDead = currentHealth <= 0;
        NotifyChanged();
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0) return;
        if (Time.time - lastDamageTime < damageCooldown) return;

        lastDamageTime = Time.time;
        float old = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        OnDamaged?.Invoke(old - currentHealth, currentHealth);
        isDead = currentHealth <= 0;
        NotifyChanged();

        if (isDead) Die();
    }

    public void Kill()
    {
        if (isDead) return;
        currentHealth = 0;
        isDead = true;
        NotifyChanged();
        Die();
    }

    private void Die()
    {
        OnDied?.Invoke();
        if (destroyOnDeath) Destroy(gameObject);
    }

    private void NotifyChanged() => OnChanged?.Invoke(currentHealth, maxHealth);
}
