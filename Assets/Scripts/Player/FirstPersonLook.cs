using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class FirstPersonLook : NetworkBehaviour
{
    [SerializeField]
    Transform character;
    public GameObject Head;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    public bool enableCameraShake = false;
    public float shakeAmount = 0.3f;

    private Vector2 velocity;
    private Vector2 frameVelocity;

    public Coroutine shakeCoroutine;

    void Reset()
    {
        if (!IsOwner) return;
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (IsOwner)
        {
            Head.SetActive(false);
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        PlayerPrefs.SetFloat("currentSensitivity", sensitivity);
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -70, 80);

        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);

        if (enableCameraShake)
        {
            if (shakeCoroutine == null)
            {
                shakeCoroutine = StartCoroutine(ShakeCamera());
            }
        }

        if (Input.GetKeyDown(KeyCode.Z)) // z для активации тряски
        {
            enableCameraShake = true;
        }

        if (enableCameraShake)
        {
            if (shakeCoroutine == null)
            {
                shakeCoroutine = StartCoroutine(ShakeCamera());
            }
        }
    }

    private IEnumerator ShakeCamera()
    {
        Vector3 originalPosition = transform.localPosition;

        float shakeTimer = 0.0f;
        float shakeDuration = Random.Range(0.5f, 1.5f);

        while (shakeTimer < shakeDuration)
        {
            float xShake = Random.Range(-shakeAmount, shakeAmount);
            float yShake = Random.Range(-shakeAmount, shakeAmount);
            transform.localPosition = originalPosition + new Vector3(xShake, yShake, 0);

            shakeTimer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }
}
