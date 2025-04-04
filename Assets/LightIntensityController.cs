using UnityEngine;

public class LightIntensityController : MonoBehaviour
{
    public Light controlledLight;
    public float maxDistance = 10f;
    public float minIntensity = 0.2f; 
    public float maxIntensity = 2f;
    public float smoothingSpeed = 5f; 
    public float hitPointSmoothing = 10f;
    public LayerMask Layer;

    private Vector3 hitPoint;
    private bool hitDetected;
    private float targetIntensity;

    void Start()
    {
        hitPoint = controlledLight.transform.position + controlledLight.transform.forward * maxDistance;
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 origin = controlledLight.transform.position;
        Vector3 direction = controlledLight.transform.forward;

        if (Physics.Raycast(origin, direction, out hit, maxDistance, Layer))
        {
            float distance = hit.distance;
            targetIntensity = Mathf.Lerp(maxIntensity, minIntensity, Mathf.InverseLerp(0f, maxDistance, distance));
            hitPoint = Vector3.Lerp(hitPoint, hit.point, Time.deltaTime * hitPointSmoothing);
            hitDetected = true;
        }
        else
        {
            targetIntensity = maxIntensity;
            hitPoint = Vector3.Lerp(hitPoint, origin + direction * maxDistance, Time.deltaTime * hitPointSmoothing);
            hitDetected = false;
        }

        controlledLight.intensity = Mathf.Lerp(controlledLight.intensity, targetIntensity, Time.deltaTime * smoothingSpeed);
    }


    void OnDrawGizmos()
    {
        if (controlledLight == null) return;

        Vector3 origin = controlledLight.transform.position;
        Vector3 direction = controlledLight.transform.forward * maxDistance;

        Gizmos.color = hitDetected ? Color.red : Color.yellow;
        Gizmos.DrawLine(origin, origin + direction);
        Gizmos.DrawSphere(origin + direction, 0.1f);
    }

}