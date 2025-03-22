using UnityEngine;
using Unity.Netcode;

public class FirstPersonLook : NetworkBehaviour
{
    [Header("Camera Settings")]
    public Camera playerCamera;
    public Transform cameraHolder;
    [SerializeField] private Transform character;
    public GameObject Head;

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

    [Header("First Person Movement Settings")]
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    private Vector2 velocity;
    private Vector2 frameVelocity;

    private Rigidbody rigidbody;
    private bool isRunning = false;

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

        rigidbody = GetComponent<Rigidbody>();

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

    void FixedUpdate()
    {
        if (!IsOwner) return;

        isRunning = Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));

        float targetMovingSpeed = isRunning ? 7f : 5f;
        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);
        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);
    }

    void HandleZoom()
    {
        bool movingFast = Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));
        float targetZoom = movingFast ? defaultZoom + 15f : defaultZoom;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetZoom, zoomSpeed * Time.deltaTime); // Плавный зум
    }

    void HandleCameraBobbing()
    {
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        if (isMoving)
        {
            float bobSpeed = isRunning ? runBobSpeed : walkBobSpeed;
            float bobAmount = isRunning ? runBobAmount : walkBobAmount;

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

        // Плавный переход к целевому углу наклона с использованием Mathf.Lerp
        float currentTilt = cameraHolder.localRotation.eulerAngles.z;
        float smoothTilt = Mathf.LerpAngle(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);

        // Применение плавного наклона
        cameraHolder.localRotation = Quaternion.Euler(0, 0, smoothTilt);
    }

    void HandleCameraShake()
    {
        if (shakeTimer > 0)
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
