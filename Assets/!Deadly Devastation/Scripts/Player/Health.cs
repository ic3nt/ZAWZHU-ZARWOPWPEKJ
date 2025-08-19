using UnityEngine;
using System;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField, Min(0)] private float _maxHealth = 100f;
    [SerializeField, Min(0)] private float _startHealth = 100f;
    [SerializeField, Tooltip("Minimum delay between damage applications (seconds)")] private float _damageCooldown = 0.1f;
    [SerializeField] private bool _destroyOnDeath = false;

    public float MaxHealth => _maxHealth;
    public float Current { get; private set; }
    public bool IsDead => Current <= 0f;

    public event Action<float, float> OnDamaged;
    public event Action<float, float> OnHealed;
    public event Action OnDied;
    public event Action<float, float> OnChanged;

    private float _lastDamageTime;

    private void Awake()
    {
        Current = Mathf.Clamp(_startHealth, 0, _maxHealth);
        _lastDamageTime = -999f;
        NotifyChanged();
    }

    public void SetMax(float newMax, bool clampToNewMax = true)
    {
        _maxHealth = Mathf.Max(0f, newMax);
        if (clampToNewMax) Current = Mathf.Min(Current, _maxHealth);
        NotifyChanged();
    }

    public void Heal(float amount)
    {
        if (amount <= 0 || IsDead) return;
        float old = Current;
        Current = Mathf.Clamp(Current + amount, 0, _maxHealth);
        OnHealed?.Invoke(Current - old, Current);
        NotifyChanged();
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0) return;
        if (Time.time - _lastDamageTime < _damageCooldown) return;
        _lastDamageTime = Time.time;

        float old = Current;
        Current = Mathf.Clamp(Current - amount, 0, _maxHealth);
        OnDamaged?.Invoke(old - Current, Current);
        NotifyChanged();

        if (Current <= 0)
            Die();
    }

    public void Kill()
    {
        if (IsDead) return;
        Current = 0;
        NotifyChanged();
        Die();
    }

    private void Die()
    {
        if (IsDead)
        {
            OnDied?.Invoke();
            if (_destroyOnDeath) Destroy(gameObject);
        }
    }

    private void NotifyChanged() => OnChanged?.Invoke(Current, _maxHealth);
}