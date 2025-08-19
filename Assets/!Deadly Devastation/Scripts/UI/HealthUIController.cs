using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUIController : MonoBehaviour
{
    [Header("Bindings")]
    [SerializeField] private Health _health;
    [SerializeField] private Image _healthBar;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private Animator _uiAnimator;

    [Header("Thresholds")]
    [SerializeField, Range(0, 100)] private float _t50 = 50;
    [SerializeField, Range(0, 100)] private float _t30 = 30;
    [SerializeField, Range(0, 100)] private float _t15 = 15;

    private static readonly int HashNormal = Animator.StringToHash("Normal");
    private static readonly int HashH50 = Animator.StringToHash("Health50");
    private static readonly int HashH30 = Animator.StringToHash("Health30");
    private static readonly int HashH15 = Animator.StringToHash("Health15");
    private static readonly int HashDead = Animator.StringToHash("Dead");

    private void OnEnable()
    {
        if (_health)
        {
            _health.OnChanged += HandleChanged;
            _health.OnDied += HandleDead;
            HandleChanged(_health.Current, _health.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (_health)
        {
            _health.OnChanged -= HandleChanged;
            _health.OnDied -= HandleDead;
        }
    }

    private void HandleChanged(float current, float max)
    {
        float pct = max > 0 ? current / max : 0f;
        if (_healthBar) _healthBar.fillAmount = pct;
        if (_healthText) _healthText.text = Mathf.RoundToInt(pct * 100f) + "%";
        UpdateThresholdAnim(current);
    }

    private void UpdateThresholdAnim(float current)
    {
        if (!_uiAnimator) return;
        if (current <= 0) return; // dead handled separately

        if (current > _t50)
            _uiAnimator.SetTrigger(HashNormal);
        else if (current > _t30)
            _uiAnimator.SetTrigger(HashH50);
        else if (current > _t15)
            _uiAnimator.SetTrigger(HashH30);
        else
            _uiAnimator.SetTrigger(HashH15);
    }

    private void HandleDead()
    {
        if (_uiAnimator) _uiAnimator.SetTrigger(HashDead);
    }
}