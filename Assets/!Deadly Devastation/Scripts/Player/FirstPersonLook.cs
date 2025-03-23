using UnityEngine;
using Unity.Netcode;

public class FirstPersonLook : NetworkBehaviour
{
    [Header("Camera Settings")]
    public Camera playerCamera;
    public Transform cameraHolder;
    [SerializeField] private Transform character;
    public GameObject Head;
    public FirstPersonMovement firstPersonMovement;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    [Header("Camera Effects")]
    public float walkBobSpeed = 8f;
    public float walkBobAmount = 0.05f;
    public float runBobSpeed = 12f;
    public float runBobAmount = 0.1f;
    private float bobTimer = 0f;
    private Vector3 initialCameraLocalPos;

    [Space(10)]
    public float tiltAngle = 5f;
    public float tiltSpeed = 5f;

    [Space(10)]
    public float shakeIntensity = 0.05f;
    public float shakeDuration = 0.2f;
    private float shakeTimer = 0f;

    [Space(10)]
    private float defaultZoom;
    public float zoomSpeed = 1f;

    private Vector3 initialHolderLocalPos;
    private Quaternion initialHolderLocalRot;
    private Vector2 velocity;
    private Vector2 frameVelocity;

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

        // Инициализируем переменные
        if (IsOwner)
        {
            if (Head != null)
            {
                Head.SetActive(false); // Скрываем голову у владельца
            }
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        HandleZoom();
        HandleCameraBobbing();
        HandleCameraTilt();
        HandleCameraShake();
        HandleLookRotation();
    }

    void HandleZoom()
    {
        float targetZoom = firstPersonMovement.IsRunning ? defaultZoom + 15f : defaultZoom;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetZoom, zoomSpeed * Time.deltaTime);
    }

    void HandleCameraBobbing()
    {
        if (firstPersonMovement.IsMoving)
        {
            float bobSpeed = firstPersonMovement.IsRunning ? runBobSpeed : walkBobSpeed;
            float bobAmount = firstPersonMovement.IsRunning ? runBobAmount : walkBobAmount;

            bobTimer += Time.deltaTime * bobSpeed;
            Vector3 bobOffset = new Vector3(0, Mathf.Sin(bobTimer) * bobAmount, 0);
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, initialCameraLocalPos + bobOffset, Time.deltaTime * 10f); // Плавное движение при боббинге
        }
        else
        {
            bobTimer = 0f;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, initialCameraLocalPos, Time.deltaTime * 10f); // Плавный возврат к изначальной позиции
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

            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, initialHolderLocalPos + shakeOffset, Time.deltaTime * 15f); // Плавная тряска
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, initialHolderLocalPos, Time.deltaTime * 5f); // Плавное возвращение к начальной позиции
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
}
