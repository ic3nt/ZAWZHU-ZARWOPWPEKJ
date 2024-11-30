using UnityEngine;

public class CameraFOVDecibelsAudioUpdater : MonoBehaviour
{
    public Camera camera;
    public AudioSource audioSource;
    public float maxFOV = 100f;
    public float minFOV = 60f;
    public float smoothSpeed = 0.1f;
    public float bassThreshold = -30f;
    private float targetFOV;
    private float originalFOV;

    void Start()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        originalFOV = camera.fieldOfView; 
        targetFOV = originalFOV; 
    }

    void Update()
    {
        float bassDBValue = GetBassDBValue();

        if (bassDBValue > bassThreshold)
        {
            float normalizedBassVolume = Mathf.Clamp01((bassDBValue + 100f) / 100f);
            targetFOV = Mathf.Lerp(originalFOV, maxFOV, normalizedBassVolume);
        }
        else
        {
            targetFOV = originalFOV;
        }

        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, Time.deltaTime / smoothSpeed);
    }

    private float GetBassDBValue()
    {
        float[] samples = new float[256];
        audioSource.GetOutputData(samples, 0);
        float currentBassVolume = 0f;

        for (int i = 0; i < samples.Length / 2; i++) 
        {
            currentBassVolume += Mathf.Abs(samples[i]);
        }

        if (currentBassVolume == 0)
            return -100f;

        return 20 * Mathf.Log10(currentBassVolume / (samples.Length / 2));
    }
}
