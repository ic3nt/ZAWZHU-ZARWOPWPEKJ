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
    private Vector3 barInitialPos;
    private Vector3 textInitialPos;
    private Vector3 barInitialScale;
    private Vector3 textInitialScale;

    private void Start()
    {
        if (!health) health = GetComponentInParent<Health>();

        health.OnChanged += UpdateBars;
        health.OnDamaged += OnDamaged;
        health.OnHealed += OnHealed;
        health.OnDied += OnDied;

        SetBarsInstant(health.Current, health.MaxHealth);

        billboard = GetComponentInChildren<WorldSpaceCanvasBillboard>();

        if (barContainer)
        {
            barInitialPos = barContainer.localPosition;
            barInitialScale = barContainer.localScale;
        }
        if (textContainer)
        {
            textInitialPos = textContainer.localPosition;
            textInitialScale = textContainer.localScale;
        }
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
            barContainer.DOKill();
            barContainer.localPosition = barInitialPos;
            barContainer
                .DOPunchPosition(punchVector, shakeDuration, 2, 0.4f)
                .OnComplete(() => barContainer.DOLocalMove(barInitialPos, 0.2f).SetEase(Ease.OutQuad));
        }

        if (textContainer)
        {
            textContainer.DOKill();
            textContainer.localPosition = textInitialPos;
            textContainer
                .DOPunchPosition(punchVector * 0.5f, shakeDuration, 2, 0.4f)
                .OnComplete(() => textContainer.DOLocalMove(textInitialPos, 0.2f).SetEase(Ease.OutQuad));
        }
    }

    private void OnHealed(float healed, float current)
    {
        Vector3 punchScale = Vector3.one * 0.1f;

        if (barContainer)
        {
            barContainer.DOKill();
            barContainer.localScale = barInitialScale;
            barContainer
                .DOPunchScale(punchScale, 0.3f, 5, 0.5f)
                .OnComplete(() => barContainer.DOScale(barInitialScale, 0.2f).SetEase(Ease.OutQuad));
        }

        if (textContainer)
        {
            textContainer.DOKill();
            textContainer.localScale = textInitialScale;
            textContainer
                .DOPunchScale(punchScale, 0.3f, 5, 0.5f)
                .OnComplete(() => textContainer.DOScale(textInitialScale, 0.2f).SetEase(Ease.OutQuad));
        }
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
