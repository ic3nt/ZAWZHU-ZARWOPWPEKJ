using UnityEngine;

public class SpeedLinesController : MonoBehaviour
{
    private ParticleSystem speedLines;
    private PlayerMovement movement;
    [SerializeField] private float maxEmissionRate = 150f;
    [SerializeField] private float lerpSpeed = 5f; 

    private ParticleSystem.EmissionModule emission;
    private float currentRate;
    private PlayerContext _ctx;

    private void Awake()
    {
        _ctx = GetComponentInParent<PlayerContext>();
        movement = _ctx.Movement;
        if (!speedLines) speedLines = GetComponent<ParticleSystem>();
        emission = speedLines.emission;

    }

    private void Update()
    {
        if (!movement) return;

        float targetRate = movement.IsRunning
            ? Mathf.Lerp(0, maxEmissionRate, movement.RunProgress02)
            : 0f;

        currentRate = Mathf.Lerp(currentRate, targetRate, Time.deltaTime * lerpSpeed);

        emission.rateOverTime = currentRate;
    }
}
