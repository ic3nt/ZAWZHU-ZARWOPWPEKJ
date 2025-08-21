using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsUIController : MonoBehaviour
{
    [Header("Bindings")]
    [SerializeField] private Health _health;
    [SerializeField] private Image _healthBar;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private Image _speedometerImage;
    [SerializeField] private TextMeshProUGUI _speedometerText;
    [SerializeField] private Animator _uiAnimator;

    [Header("Thresholds")]
    [SerializeField, Range(0, 100)] private float _t50 = 50;
    [SerializeField, Range(0, 100)] private float _t30 = 30;
    [SerializeField, Range(0, 100)] private float _t15 = 15;

    private float maxShakeStrength = 30f;
    private float maxSpeedForShake = 15f;
    private float baseSmoothTime = 0.3f;
    private float minNoiseFrequency = 5f;
    private float maxNoiseFrequency = 25f;

    private static readonly int HashNormal = Animator.StringToHash("Normal");
    private static readonly int HashH50 = Animator.StringToHash("Health50");
    private static readonly int HashH30 = Animator.StringToHash("Health30");
    private static readonly int HashH15 = Animator.StringToHash("Health15");
    private static readonly int HashDead = Animator.StringToHash("Dead");

    private PlayerContext _ctx;
    private float _currentShakeStrength;
    private Vector2 _originalPos;

    private void Awake()
    {
        _ctx = GetComponentInParent<PlayerContext>();
        if (_speedometerImage != null)
            _originalPos = _speedometerImage.rectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (_ctx == null || _ctx.Movement == null) return;

        int speed = Mathf.RoundToInt(_ctx.Movement.CurrentHorizontalSpeed);
        if (_speedometerText != null)
            _speedometerText.text = speed.ToString();

        float speedRatio = Mathf.Clamp01(speed / maxSpeedForShake);
        float targetStrength = speedRatio * maxShakeStrength;
        float dynamicSmooth = Mathf.Lerp(baseSmoothTime * 2f, baseSmoothTime, speedRatio);
        float dynamicFrequency = Mathf.Lerp(minNoiseFrequency, maxNoiseFrequency, speedRatio);

        _currentShakeStrength = Mathf.Lerp(_currentShakeStrength, targetStrength, Time.deltaTime / dynamicSmooth);

        if (_speedometerImage != null)
        {
            if (_currentShakeStrength > 0.01f)
            {
                float time = Time.time * dynamicFrequency;
                float offsetX = (Mathf.PerlinNoise(time, 0f) - 0.5f) * 2f * _currentShakeStrength;
                float offsetY = (Mathf.PerlinNoise(0f, time) - 0.5f) * 2f * _currentShakeStrength;
                _speedometerImage.rectTransform.anchoredPosition = _originalPos + new Vector2(offsetX, offsetY);
            }
            else
            {
                _speedometerImage.rectTransform.anchoredPosition = _originalPos;
            }
        }
    }

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.OnChanged += HandleChanged;
            _health.OnDied += HandleDead;
            HandleChanged(_health.Current, _health.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnChanged -= HandleChanged;
            _health.OnDied -= HandleDead;
        }
    }

    private void HandleChanged(float current, float max)
    {
        float pct = max > 0 ? current / max : 0f;
        if (_healthBar != null) _healthBar.fillAmount = pct;
        if (_healthText != null) _healthText.text = Mathf.RoundToInt(pct * 100f) + "%";
        UpdateThresholdAnim(current);
    }

    private void UpdateThresholdAnim(float current)
    {
        if (_uiAnimator == null) return;
        if (current <= 0) return;

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
        if (_uiAnimator != null) _uiAnimator.SetTrigger(HashDead);
    }
}
