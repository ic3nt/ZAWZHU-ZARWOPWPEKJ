using UnityEngine;
using Unity.Netcode;

public class FirstPersonLook : NetworkBehaviour
{
    [Header("Camera Settings")]
    [Space(10)]
    public Camera playerCamera;
    public Transform cameraHolder;
    [SerializeField] private Transform character;
    public GameObject Head;
    public FirstPersonMovement firstPersonMovement;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    private Vector3 initialHolderLocalPos;
    private Quaternion initialHolderLocalRot;
    private Vector2 velocity;
    private Vector2 frameVelocity;
    private Vector3 initialCameraPosition;

    [Header("Camera Bob")]
    [Space(10)]
    public float walkBobSpeed = 8f;
    public float walkBobAmount = 0.05f;
    public float runBobSpeed = 12f;
    public float runBobAmount = 0.1f;
    private float bobTimer = 0f;
    private Vector3 initialCameraLocalPos;

    [Header("Camera Tilt")]
    [Space(10)]
    public float tiltAngle = 5f;
    public float tiltSpeed = 5f;

    [Header("Camera Shake")]
    [Space(10)]
    public float shakeIntensity = 0.05f;
    public float shakeDuration = 0.2f;
    private float shakeTimer = 0f;

    [Header("Player Run Zoom")]
    [Space(10)]
    private float defaultZoom;
    public float zoomSpeed = 1f;
    public float zoomInFOV = 30f; 

    [Header("Camera Enemy Focus Settings")]
    [Space(10)]
    public bool canFocusOnEnemy = true;
    public float focusSpeed = 5f;
    public float focusDistance = 10f;
    [SerializeField] private Transform currentFocusTarget;

    public AudioClip enemyDetectedSound;
    public AudioSource audioSource;

    public bool enableShakeOnFocus = true;
    public float focusShakeIntensity = 0.1f;
    public float focusShakeDuration = 0.3f;

    private bool isZoomedIn = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        initialCameraLocalPos = playerCamera.transform.localPosition;
        initialHolderLocalPos = cameraHolder.localPosition;
        initialHolderLocalRot = cameraHolder.localRotation;
        defaultZoom = playerCamera.fieldOfView;

        if (!IsOwner)
        {
            playerCamera.enabled = false;
            return;
        }

        if (IsOwner)
        {
            if (Head != null)
            {
                Head.SetActive(false);
            }
        }

        initialCameraPosition = playerCamera.transform.position;
    }

    void Update()
    {
        if (!IsOwner) return;

        HandleZoom();
        HandleCameraBobbing();
        HandleCameraTilt();
        HandleCameraShake();
        HandleLookRotation();

        if (canFocusOnEnemy)
        {
            CheckForEnemyInView();
            FocusOnEnemy();
        }
    }

    void HandleZoom()
    {
        if (isZoomedIn)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomInFOV, zoomSpeed * Time.deltaTime);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, defaultZoom, zoomSpeed * Time.deltaTime);
        }
    }

    void HandleCameraBobbing()
    {
        if (firstPersonMovement.IsMoving)
        {
            float bobSpeed = firstPersonMovement.IsRunning ? runBobSpeed : walkBobSpeed;
            float bobAmount = firstPersonMovement.IsRunning ? runBobAmount : walkBobAmount;

            bobTimer += Time.deltaTime * bobSpeed;
            Vector3 bobOffset = new Vector3(0, Mathf.Sin(bobTimer) * bobAmount, 0);
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, initialCameraLocalPos + bobOffset, Time.deltaTime * 10f); // Smooth bobbing
        }
        else
        {
            bobTimer = 0f;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, initialCameraLocalPos, Time.deltaTime * 10f); // Smooth return to initial position
        }
    }

    void HandleCameraTilt()
    {
        float targetTilt = 0f;

        if (Input.GetKey(KeyCode.A))
            targetTilt = tiltAngle;
        else if (Input.GetKey(KeyCode.D))
            targetTilt = -tiltAngle;

        float currentTilt = cameraHolder.localRotation.eulerAngles.z;
        float smoothTilt = Mathf.LerpAngle(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);

        cameraHolder.localRotation = Quaternion.Euler(0, 0, smoothTilt);
    }

    void HandleCameraShake()
    {
        if (shakeTimer > 0 && firstPersonMovement.IsRunning)
        {
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0
            );

            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, initialHolderLocalPos + shakeOffset, Time.deltaTime * 15f);
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, initialHolderLocalPos, Time.deltaTime * 5f);
        }

        if (enableShakeOnFocus && currentFocusTarget != null)
        {
            Vector3 shakeOffset = new Vector3(
                Random.Range(-focusShakeIntensity, focusShakeIntensity),
                Random.Range(-focusShakeIntensity, focusShakeIntensity),
                0
            );

            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, initialHolderLocalPos + shakeOffset, Time.deltaTime * 20f);
        }
    }

    void HandleLookRotation()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -70, 80);

        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }

    public void TriggerShake(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeDuration = duration;
        shakeTimer = duration;
    }

    void CheckForEnemyInView()
    {
        RaycastHit hit;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Enemy") && hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (!isZoomedIn)
                {
                    isZoomedIn = true;
                }

                currentFocusTarget = hit.collider.transform;

                if (!audioSource.isPlaying && enemyDetectedSound != null)
                {
                    audioSource.PlayOneShot(enemyDetectedSound);
                }
            }
            else
            {
                if (isZoomedIn)
                {
                    isZoomedIn = false;
                    currentFocusTarget = null;
                }
            }
        }
    }

    void FocusOnEnemy()
    {
        if (currentFocusTarget != null)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomInFOV, zoomSpeed * Time.deltaTime);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, defaultZoom, zoomSpeed * Time.deltaTime);
        }
    }
}
