using UnityEngine;

public class CameraFOVDecibelsAudioUpdater : MonoBehaviour
{
    public Camera camera; // Ссылка на вашу камеру
    public AudioSource audioSource; // Ссылка на источник аудио
    public float maxFOV = 100f; // Максимальное значение FOV
    public float minFOV = 60f; // Минимальное значение FOV
    public float smoothSpeed = 0.1f; // Скорость изменения FOV
    public float bassThreshold = -30f; // Порог для изменения FOV на основе басов
    private float targetFOV; // Целевое значение FOV
    private float originalFOV; // Оригинальное значение FOV

    void Start()
    {
        if (camera == null)
        {
            camera = Camera.main; // Использовать основную камеру, если не указана
        }
        originalFOV = camera.fieldOfView; // Сохранить оригинальное значение FOV
        targetFOV = originalFOV; // Установить целевое значение FOV на оригинальное
    }

    void Update()
    {
        // Получаем уровень баса в децебелах
        float bassDBValue = GetBassDBValue();

        // Если уровень громкости баса превышает порог, меняем FOV
        if (bassDBValue > bassThreshold)
        {
            // Преобразуем уровень громкости в значение FOV
            float normalizedBassVolume = Mathf.Clamp01((bassDBValue + 100f) / 100f); // Приведение к диапазону [0, 1]
            targetFOV = Mathf.Lerp(originalFOV, maxFOV, normalizedBassVolume); // Линейная интерполяция для FOV
        }
        else
        {
            targetFOV = originalFOV; // Вернуться к оригинальному значению FOV если под порогом
        }

        // Плавная смена значения FOV
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, Time.deltaTime / smoothSpeed);
    }

    // Метод для получения уровня баса в децебелах
    private float GetBassDBValue()
    {
        float[] samples = new float[256];
        audioSource.GetOutputData(samples, 0);
        float currentBassVolume = 0f;

        // Для получения баса, мы будем учитывать только низкие частоты
        for (int i = 0; i < samples.Length / 2; i++) // Обработка первой половины массива для низких частот
        {
            currentBassVolume += Mathf.Abs(samples[i]);
        }

        // Проверяем, чтобы мы не делили на ноль
        if (currentBassVolume == 0)
            return -100f;

        return 20 * Mathf.Log10(currentBassVolume / (samples.Length / 2)); // Дивидируем на 2, чтобы получить среднее только по басам
    }
}
