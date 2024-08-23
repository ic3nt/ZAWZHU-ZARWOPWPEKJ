using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Определите параметры для тряски камеры
    [Header("Shake Settings")]
    public float shakeDuration = 1f; // Длительность тряски
    public float shakeMagnitude = 0.2f; // Амплитуда тряски
    public float shakeFrequency = 1f; // Частота тряски

    private Vector3 originalPosition; // Исходное положение камеры
    private float shakeTime = 0f; // Время тряски

    void Start()
    {
        originalPosition = transform.localPosition; // Сохраняем исходную позицию
    }

    void Update()
    {
        if (shakeTime > 0)
        {
            // Посчитываем, сколько времени осталось до окончания тряски
            shakeTime -= Time.deltaTime;

            // Вычисляем новое смещение в зависимости от времени и частоты
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            // Применяем сдвиг к камере
            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            // Если время, отведенное на тряску, закончилось, сбрасываем смещение
            if (shakeTime <= 0)
            {
                shakeTime = 0;
                transform.localPosition = originalPosition; // Возвращаем в исходное положение
            }
        }
    }

    // Публичный метод для начала тряски камеры
    public void ShakeCamera()
    {
        shakeTime = shakeDuration; // Устанавливаем время тряски
    }
}
