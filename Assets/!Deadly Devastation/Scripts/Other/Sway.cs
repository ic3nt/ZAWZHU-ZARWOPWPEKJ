using UnityEngine;

public class Sway : MonoBehaviour
{
    [SerializeField] private bool enabledSway = true;
    [SerializeField] private Vector3 baseLocalPosition;
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float maxAmplitude = 5f;
    [SerializeField] private float smooth = 10f;

    private void LateUpdate()
    {
        if (!enabledSway) return;
        float mX = -Input.GetAxis("Mouse X") * amplitude;
        float mY = -Input.GetAxis("Mouse Y") * amplitude;
        mX = Mathf.Clamp(mX, -maxAmplitude, maxAmplitude);
        mY = Mathf.Clamp(mY, -maxAmplitude, maxAmplitude);
        Vector3 target = new Vector3(mX, mY, 0) + baseLocalPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * smooth);
    }
}