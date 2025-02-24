using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Manual : MonoBehaviour
{
    public RectTransform manualLabel;
    public RectTransform bg;
    public RectTransform firstWindow;
    public RectTransform secondWindow;
    public Transform target3D; // 3D-объект

    public float shakeStrength = 10f;
    public int shakeVibrato = 10;
    public float shakeDuration = 0.3f;

    public Vector3 upPosition; // Начальная позиция
    public Vector3 bottomPosition;   // Конечная позиция

    public Vector3 upRotation; // Начальное вращение (в градусах)
    public Vector3 bottomRotation;   // Конечное вращение (в градусах)

    public float moveDuration = 1f; // Длительность движения
    public float rotationDuration = 1f; // Длительность вращения

    public float shakeStrength3D = 10f;
    public int shakeVibrato3D = 10;
    public float shakeDuration3D = 0.3f;

    private void Start()
    {
        target3D.position = bottomPosition;
        target3D.rotation = Quaternion.Euler(bottomRotation);
    }

    public void WindowOpen()
    {
        float screenHeight = Screen.height;
        MoveUp();
        firstWindow.DOAnchorPosY(-screenHeight, 0.3f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                firstWindow.gameObject.SetActive(false);
                secondWindow.gameObject.SetActive(true);
                secondWindow.anchoredPosition = new Vector2(secondWindow.anchoredPosition.x, -screenHeight);

                secondWindow.DOAnchorPosY(0f, 0.3f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        manualLabel.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato);
                        secondWindow.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato);
                        bg.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato);
                    });
            });
    }

    public void WindowClose()
    {
        float screenHeight = Screen.height;
        MoveDown();
        secondWindow.DOAnchorPosY(-screenHeight, 0.3f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                secondWindow.gameObject.SetActive(false);
                firstWindow.gameObject.SetActive(true);
                firstWindow.anchoredPosition = new Vector2(secondWindow.anchoredPosition.x, -screenHeight);

                firstWindow.DOAnchorPosY(0f, 0.3f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        manualLabel.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato);
                        firstWindow.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato);
                        bg.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato);
                    });
            });
    }

    private void MoveUp()
    {
        // Более плавное движение
        target3D.DOMove(upPosition, moveDuration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // Тряска объекта в конце движения
                target3D.DOShakePosition(shakeDuration3D, shakeStrength3D, shakeVibrato3D);
            });

        // Более плавное вращение
        target3D.DORotate(upRotation, rotationDuration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // Тряска объекта в конце вращения (если нужно отдельно)
                target3D.DOShakeRotation(shakeDuration3D, shakeStrength3D, shakeVibrato3D);
            });
    }

    private void MoveDown()
    {
        // Более плавное движение
        target3D.DOMove(bottomPosition, moveDuration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // Тряска объекта в конце движения
                target3D.DOShakePosition(shakeDuration3D, shakeStrength3D, shakeVibrato3D);
            });

        // Более плавное вращение
        target3D.DORotate(bottomRotation, rotationDuration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // Тряска объекта в конце вращения (если нужно отдельно)
                target3D.DOShakeRotation(shakeDuration3D, shakeStrength3D, shakeVibrato3D);
            });
    }
}
