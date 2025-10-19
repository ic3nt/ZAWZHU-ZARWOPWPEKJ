using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public enum SelectorType
{
    Circular,
    Grid
}

public class SelectorController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private SelectorType selectorType = SelectorType.Circular;
    [SerializeField] private KeyCode openKey = KeyCode.Tab;

    [Header("References")]
    [SerializeField] private RectTransform selectorTransform;
    [SerializeField] private Image selectedIcon;
    [SerializeField] private TextMeshProUGUI centerText;
    [SerializeField] private Sprite noImage;
    [SerializeField] private List<SelectorButton> buttons = new();

    [Header("Circular Settings")]
    [SerializeField] private float radius = 200f;

    [Header("Animation Settings")]
    [SerializeField] private float openDuration = 0.4f;
    [SerializeField] private float closeDuration = 0.25f;
    [SerializeField] private Ease openEase = Ease.OutBack;
    [SerializeField] private Ease closeEase = Ease.InBack;

    [Header("Circle Outline Settings")]
    [SerializeField] private RectTransform circleOutline;
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private bool rotateClockwise = true;
    [SerializeField] private bool rotateOutline = true;
    [SerializeField] private bool animateColor = true;
    [SerializeField] private float colorCycleDuration = 3f;
    [Range(0f, 1f)]
    [SerializeField] private float brightness = 1f;

    private Image circleImage;
    private Tween rotationTween;
    private Tween colorTween;
    private Sequence animSequence;

    private bool isVisible;
    private Vector3 baseScale;
    private SelectorButton hoveredButton;

    public int SelectedItemID { get; private set; } = -1;

    private void Awake()
    {
        if (!selectorTransform)
        {
            Debug.LogError("[SelectorController] Missing selectorTransform reference!");
            enabled = false;
            return;
        }

        baseScale = selectorTransform.localScale;
        selectorTransform.localScale = Vector3.zero;

        if (circleOutline)
            circleImage = circleOutline.GetComponent<Image>();

        if (selectorType == SelectorType.Circular)
            ArrangeCircularButtons();
        else
            AssignGridButtons();

        HideInstant();
    }

    private void Start()
    {
        StartCircleRotation();
        StartCircleColorCycle();
    }

    private void Update()
    {
        if (Input.GetKeyDown(openKey)) Show();
        if (Input.GetKeyUp(openKey)) Hide();
    }


    private void StartCircleRotation()
    {
        if (!circleOutline || !rotateOutline) return;
        rotationTween?.Kill();

        float direction = rotateClockwise ? -1f : 1f;
        float duration = 360f / rotationSpeed;

        rotationTween = circleOutline
            .DOLocalRotate(new Vector3(0, 0, 360f * direction), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    private void StartCircleColorCycle()
    {
        if (!circleImage) return;
        colorTween?.Kill();

        if (!animateColor)
        {
            circleImage.color = Color.white * brightness;
            return;
        }

        colorTween = DOTween.To(() => 0f, x =>
        {
            Color rgb = Color.HSVToRGB(x % 1f, 1f, brightness);
            circleImage.color = rgb;
        }, 1f, colorCycleDuration)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart);
    }

    private void ArrangeCircularButtons()
    {
        int count = buttons.Count;
        if (count == 0) return;

        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angleDeg = i * step;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            Vector2 pos = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * radius;
            RectTransform rect = buttons[i].GetComponent<RectTransform>();

            rect.SetParent(selectorTransform, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = Vector3.one;
            rect.anchoredPosition = pos;

            float lookAngle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
            rect.localRotation = Quaternion.Euler(0f, 0f, lookAngle + 270f);

            buttons[i].Setup(this);
        }
    }

    private void AssignGridButtons()
    {
        foreach (var btn in buttons)
            btn.Setup(this);
    }

    public void Show()
    {
        if (isVisible) return;
        isVisible = true;

        animSequence?.Kill();
        selectorTransform.localScale = Vector3.zero;

        if (selectorType == SelectorType.Grid)
            AssignGridButtons();

        animSequence = DOTween.Sequence();

        animSequence.Append(selectorTransform.DOScale(baseScale, openDuration).SetEase(openEase))
            .OnStart(() =>
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (centerText)
                {
                    centerText.text = "Выберите элемент";
                    centerText.alpha = 0f;
                    centerText.DOFade(1f, 0.3f);
                }

                if (selectorType == SelectorType.Circular)
                {
                    for (int i = 0; i < buttons.Count; i++)
                    {
                        var btn = buttons[i];
                        RectTransform rect = btn.GetComponent<RectTransform>();
                        rect.localScale = Vector3.zero;

                        rect.DOScale(1f, 0.25f)
                            .SetEase(Ease.OutBack)
                            .SetDelay(0.05f * i);
                    }
                }
            });
    }

    public void Hide()
    {
        if (!isVisible) return;
        isVisible = false;

        SelectorButton selected = hoveredButton;

        if (selected)
        {
            SelectedItemID = selected.ID;
            if (selectedIcon) selectedIcon.sprite = selected.icon;

            if (centerText)
            {
                centerText.text = "OK!";
                centerText.alpha = 1f;
                centerText.transform.localScale = Vector3.one * 0.7f;

                centerText.DOFade(1f, 0.25f);
                centerText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

                DOVirtual.DelayedCall(1.2f, () =>
                {
                    if (centerText) centerText.text = "";
                });
            }

            Debug.Log($"[Selector] Selected: {selected.itemName}");
        }
        else
        {
            if (selectedIcon) selectedIcon.sprite = noImage;
            if (centerText) centerText.text = "";
        }

        animSequence?.Kill();
        animSequence = DOTween.Sequence();

        animSequence.Append(selectorTransform.DOScale(Vector3.zero, closeDuration).SetEase(closeEase))
            .OnStart(() =>
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            });

        foreach (var btn in buttons)
            btn.Deselect();

        hoveredButton = null;
    }

    private void HideInstant()
    {
        selectorTransform.localScale = Vector3.zero;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isVisible = false;
    }

    public void OnButtonHover(SelectorButton button)
    {
        if (!button || hoveredButton == button) return;

        hoveredButton?.Deselect();
        hoveredButton = button;
        hoveredButton.Select();

        if (centerText)
        {
            centerText.text = button.itemName;
            centerText.DOFade(1f, 0.2f);
        }
    }

    public void OnButtonClick(SelectorButton button)
    {
        if (!button) return;
        hoveredButton = button;
        Hide();
    }
}
