using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class WheelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform wheelTransform;
    [SerializeField] private Image selectedIcon;
    [SerializeField] private TextMeshProUGUI centerText;
    [SerializeField] private Sprite noImage;
    [SerializeField] private List<WheelButton> buttons = new();

    [Header("Wheel Settings")]
    [SerializeField] private float radius = 200f;
    [SerializeField] private float openDuration = 0.4f;
    [SerializeField] private float closeDuration = 0.25f;
    [SerializeField] private Ease openEase = Ease.OutBack;
    [SerializeField] private Ease closeEase = Ease.InBack;

    private bool isVisible;
    private Vector3 baseScale;
    private Sequence wheelSequence;
    private WheelButton hoveredButton;

    public int SelectedItemID { get; private set; } = -1;

    private void Awake()
    {
        if (!wheelTransform)
        {
            Debug.LogError("[WheelController] Missing wheelTransform reference!");
            enabled = false;
            return;
        }

        baseScale = wheelTransform.localScale;
        wheelTransform.localScale = Vector3.zero;

        ArrangeButtons();
        HideInstant();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) Show();
        if (Input.GetKeyUp(KeyCode.Tab)) Hide();
    }

    private void ArrangeButtons()
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
            rect.SetParent(wheelTransform, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = Vector3.one;
            rect.anchoredPosition = pos;


            float lookAngle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
            rect.localRotation = Quaternion.Euler(0f, 0f, lookAngle + 270f);

            buttons[i].Setup(this, angleDeg);
        }
    }




    public void Show()
    {
        if (isVisible) return;
        isVisible = true;

        wheelSequence?.Kill();
        wheelTransform.localScale = Vector3.zero;

        wheelSequence = DOTween.Sequence();

        wheelSequence.Append(wheelTransform.DOScale(baseScale, openDuration).SetEase(openEase))
            .OnStart(() =>
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (centerText)
                {
                    centerText.text = "Выберите эмоцию";
                    centerText.alpha = 0f;
                    centerText.DOFade(1f, 0.3f);
                }

                // поочередное появление кнопок
                for (int i = 0; i < buttons.Count; i++)
                {
                    var btn = buttons[i];
                    RectTransform rect = btn.GetComponent<RectTransform>();
                    rect.localScale = Vector3.zero;

                    rect.DOScale(1f, 0.25f)
                        .SetEase(Ease.OutBack)
                        .SetDelay(0.05f * i);
                }
            });
    }

    public void Hide()
    {
        if (!isVisible) return;
        isVisible = false;

        WheelButton selected = hoveredButton;

        if (selected)
        {
            SelectedItemID = selected.ID;
            if (selectedIcon) selectedIcon.sprite = selected.icon;

            if (centerText)
            {
                centerText.text = "OKAY!";
                centerText.alpha = 1f;
                centerText.transform.localScale = Vector3.one * 0.7f;

                centerText.DOFade(1f, 0.25f);
                centerText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

                DOVirtual.DelayedCall(1.3f, () =>
                {
                    if (centerText) centerText.text = "";
                });
            }

            Debug.Log($"Selected emotion: {selected.itemName}");
        }
        else
        {
            if (selectedIcon) selectedIcon.sprite = noImage;
            if (centerText) centerText.text = "";
        }

        wheelSequence?.Kill();
        wheelSequence = DOTween.Sequence();

        wheelSequence.Append(wheelTransform.DOScale(Vector3.zero, closeDuration).SetEase(closeEase))
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
        wheelTransform.localScale = Vector3.zero;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isVisible = false;
    }

    public void OnButtonHover(WheelButton button)
    {
        if (!button || hoveredButton == button) return;

        hoveredButton?.Deselect();
        hoveredButton = button;
        hoveredButton.Select();

        if (centerText) centerText.text = button.itemName;
    }

    public void OnButtonClick(WheelButton button)
    {
        if (!button) return;
        hoveredButton = button;
        Hide();
    }
}
