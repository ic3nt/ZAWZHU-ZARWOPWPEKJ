using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(WorldSpaceCanvasBillboard))]
public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Health health;
    [SerializeField] Image frontBar;
    [SerializeField] Image backBar;
    [SerializeField] RectTransform barContainer;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] RectTransform textContainer;

    [Header("Animation Settings")]
    [SerializeField] float frontSpeed = 1f;
    [SerializeField] float backSpeed = 2f;
    [SerializeField] float shakeDuration = 1f;
    [SerializeField] float shakeStrength = 0.3f;

    WorldSpaceCanvasBillboard billboard;
    Vector3 barInitialPos;
    Vector3 textInitialPos;
    Vector3 barInitialScale;
    Vector3 textInitialScale;
    bool shown = false;

    void Start()
    {
        if (!health) health = GetComponentInParent<Health>();

        health.OnChanged += UpdateBars;
        health.OnDamaged += OnDamaged;
        health.OnHealed += OnHealed;
        health.OnDied += OnDied;

        SetBarsInstant(health.Current, health.MaxHealth);

        billboard = GetComponent<WorldSpaceCanvasBillboard>();
        billboard.HideImmediateAndDisableAutoFade();

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

    void UpdateBars(float current, float max)
    {
        float targetFill = max > 0f ? current / max : 0f;
        frontBar.DOFillAmount(targetFill, frontSpeed).SetEase(Ease.OutCubic);
        backBar.DOFillAmount(targetFill, backSpeed).SetEase(Ease.OutCubic);
        if (hpText) hpText.text = $"{Mathf.CeilToInt(current)}";
    }

    void OnDamaged(float damage, float current)
    {
        if (!shown)
        {
            billboard.FadeInAndEnableAutoFade(0.5f);
            shown = true;
        }

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

    void OnHealed(float healed, float current)
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

    void OnDied()
    {
        billboard.FadeOutAndDisableAutoFade(1.5f);
        shown = false;
    }

    void SetBarsInstant(float current, float max)
    {
        float fill = max > 0f ? current / max : 0f;
        frontBar.fillAmount = fill;
        backBar.fillAmount = fill;
        if (hpText) hpText.text = $"{Mathf.CeilToInt(current)}";
    }
}
