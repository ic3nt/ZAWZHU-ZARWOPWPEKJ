using UnityEngine;

public class UIBassShakeWithRotation : MonoBehaviour
{
    public AudioSource audioSource;
    public RectTransform uiElement;

    [Header("Bass Detection")]
    public float sensitivity = 50f;
    public float beatThreshold = 0.2f;
    public float beatCooldown = 0.1f;

    [Header("Shake Settings")]
    public float shakeAmount = 10f;
    public float returnSpeed = 5f;

    [Header("Rotation Settings")]
    public float maxRotationZ = 15f;

    private float[] spectrumData = new float[64];
    private Vector2 originalPos;
    private Vector2 targetOffset;
    private float currentZRotation = 0f;
    private float targetZRotation = 0f;
    private float cooldownTimer = 0f;

    void Start()
    {
        if (uiElement == null)
            uiElement = GetComponent<RectTransform>();

        originalPos = uiElement.anchoredPosition;
    }

    void Update()
    {
        // Cooldown timer
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        // Analyze bass frequencies
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
        float bass = 0f;
        for (int i = 0; i < 6; i++)
            bass += spectrumData[i];

        bass *= sensitivity;

        if (bass > beatThreshold && cooldownTimer <= 0f)
        {
            cooldownTimer = beatCooldown;

            // Apply random shake and rotation
            targetOffset = Random.insideUnitCircle * shakeAmount;
            targetZRotation = Random.Range(-maxRotationZ, maxRotationZ);
        }

        // Smooth position
        Vector2 currentOffset = Vector2.Lerp(uiElement.anchoredPosition - originalPos, targetOffset, Time.deltaTime * returnSpeed);
        uiElement.anchoredPosition = originalPos + currentOffset;
        targetOffset = Vector2.Lerp(targetOffset, Vector2.zero, Time.deltaTime * returnSpeed);

        // Smooth rotation
        currentZRotation = Mathf.Lerp(currentZRotation, targetZRotation, Time.deltaTime * returnSpeed);
        uiElement.localRotation = Quaternion.Euler(0f, 0f, currentZRotation);
        targetZRotation = Mathf.Lerp(targetZRotation, 0f, Time.deltaTime * returnSpeed);
    }
}
