using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(WorldSpaceCanvasBillboard))]
public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Image frontBar;
    [SerializeField] private Image backBar;
    [SerializeField] private RectTransform barContainer;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private RectTransform textContainer;

    [Header("Animation Settings")]
    [SerializeField] private float frontSpeed = 1f;
    [SerializeField] private float backSpeed = 2f;
    [SerializeField] private float shakeDuration = 1f;
    [SerializeField] private float shakeStrength = 0.3f;

    private WorldSpaceCanvasBillboard billboard;

    private void Start()
    {
        if (!health) health = GetComponentInParent<Health>();

        health.OnChanged += UpdateBars;
        health.OnDamaged += OnDamaged;
        health.OnHealed += OnHealed;
        health.OnDied += OnDied;

        SetBarsInstant(health.Current, health.MaxHealth);

        billboard = GetComponentInChildren<WorldSpaceCanvasBillboard>();
    }

    private void UpdateBars(float current, float max)
    {
        float targetFill = current / max;
        frontBar.DOFillAmount(targetFill, frontSpeed).SetEase(Ease.OutCubic);
        backBar.DOFillAmount(targetFill, backSpeed).SetEase(Ease.OutCubic);

        if (hpText)
            hpText.text = $"{Mathf.CeilToInt(current)}";
    }

    private void OnDamaged(float damage, float current)
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized * shakeStrength;
        Vector3 punchVector = new Vector3(randomDir.x, randomDir.y, 0f);

        if (barContainer)
        {
            barContainer.DOPunchPosition(
                punchVector,
                shakeDuration,
                2,
                0.4f
            );
        }

        if (textContainer)
        {
            textContainer.DOPunchPosition(
                punchVector * 0.5f,
                shakeDuration,
                2,
                0.4f
            );
        }
    }

    private void OnHealed(float healed, float current)
    {
        Vector3 punchScale = Vector3.one * 0.1f;

        if (barContainer)
            barContainer.DOPunchScale(punchScale, 0.3f, 5, 0.5f);

        if (textContainer)
            textContainer.DOPunchScale(punchScale, 0.3f, 5, 0.5f);
    }

    private void OnDied()
    {
        billboard.FadeOut(1.5f);
    }


    private void SetBarsInstant(float current, float max)
    {
        float fill = current / max;
        frontBar.fillAmount = fill;
        backBar.fillAmount = fill;

        if (hpText)
            hpText.text = $"{Mathf.CeilToInt(current)}";
    }
}
