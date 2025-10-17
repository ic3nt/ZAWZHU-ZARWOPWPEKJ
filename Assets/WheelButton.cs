using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class WheelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Button Info")]
    public int ID;
    public string itemName;
    public Sprite icon;

    [Header("UI References")]
    [SerializeField] private Image buttonIcon;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private Image outlineEffect;

    [Header("Outline Settings")]
    [SerializeField, Range(0f, 1f)] private float outlineVisibleAlpha = 1f;
    [SerializeField, Range(0f, 1f)] private float outlineHiddenAlpha = 0f;
    [SerializeField] private float outlineFadeIn = 0.25f;
    [SerializeField] private float outlineFadeOut = 0.5f;

    [Header("Icon Hover Animation")]
    [SerializeField] private float hoverLift = 10f;     
    [SerializeField] private float hoverScale = 1.1f;   
    [SerializeField] private float hoverInDuration = 0.2f;
    [SerializeField] private float hoverOutDuration = 0.25f;
    [SerializeField] private float shakeStrength = 5f;     
    [SerializeField] private float shakeDuration = 1.2f;  

    private RectTransform rect;
    private Vector3 startScale;
    private Vector2 startIconPos;
    private WheelController controller;

    private Tween fadeTween;
    private Tween hoverTween;
    private Tween shakeTween;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        startScale = rect.localScale;

        if (buttonIcon)
        {
            if (icon) buttonIcon.sprite = icon;
            startIconPos = buttonIcon.rectTransform.anchoredPosition;
        }

        if (itemText)
            itemText.text = "";

        if (outlineEffect)
        {
            var c = outlineEffect.color;
            c.a = outlineHiddenAlpha;
            outlineEffect.color = c;
        }
    }

    public void Setup(WheelController ctrl, float angleDeg)
    {
        controller = ctrl;

        if (buttonIcon)
            buttonIcon.rectTransform.localRotation = Quaternion.identity;

        if (itemText)
            itemText.rectTransform.localRotation = Quaternion.identity;

        if (outlineEffect)
            outlineEffect.rectTransform.localRotation = Quaternion.identity;
    }


    public void Select()
    {
        if (itemText)
            itemText.text = itemName;
    }

    public void Deselect()
    {
        if (itemText)
            itemText.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        controller?.OnButtonHover(this);
        ActivateOutline(true);
        AnimateIconHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ActivateOutline(false);
        AnimateIconHover(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        controller?.OnButtonClick(this);
    }

    private void ActivateOutline(bool active)
    {
        if (outlineEffect == null) return;

        outlineEffect.DOKill();
        fadeTween?.Kill();

        if (active)
        {
            fadeTween = outlineEffect.DOFade(outlineVisibleAlpha, outlineFadeIn)
                .SetEase(Ease.OutQuad);
        }
        else
        {
            fadeTween = outlineEffect.DOFade(outlineHiddenAlpha, outlineFadeOut)
                .SetEase(Ease.InOutQuad);
        }
    }

    private void AnimateIconHover(bool hovering)
    {
        if (buttonIcon == null) return;

        var iconRect = buttonIcon.rectTransform;
        iconRect.DOKill();
        hoverTween?.Kill();
        shakeTween?.Kill();

        if (hovering)
        {
            hoverTween = DOTween.Sequence()
                .Join(iconRect.DOAnchorPosY(startIconPos.y + hoverLift, hoverInDuration).SetEase(Ease.OutQuad))
                .Join(iconRect.DOScale(hoverScale, hoverInDuration).SetEase(Ease.OutBack));


            shakeTween = DOTween.Sequence()
                // Лёгкое дыхание (пульсация)
                .Append(iconRect.DOScale(hoverScale * 1.03f, 0.6f).SetEase(Ease.InOutSine))
                .Append(iconRect.DOScale(hoverScale * 0.97f, 0.6f).SetEase(Ease.InOutSine))
                .SetLoops(-1, LoopType.Yoyo);

            iconRect.DOShakeAnchorPos(
                duration: shakeDuration,
                strength: new Vector2(shakeStrength * 0.5f, shakeStrength),
                vibrato: 8,
                randomness: 90f,
                fadeOut: false)
                .SetLoops(-1, LoopType.Restart);
        }
        else
        {
            hoverTween = DOTween.Sequence()
                .Join(iconRect.DOAnchorPos(startIconPos, hoverOutDuration).SetEase(Ease.OutQuad))
                .Join(iconRect.DOScale(1f, hoverOutDuration).SetEase(Ease.OutBack));

            shakeTween?.Kill();
        }
    }

}
