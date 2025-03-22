using UnityEngine;
using Unity.Netcode;

public class FirstPersonMovement : NetworkBehaviour
{
    public float speed = 5;
    public Animator animator;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 7;
    public KeyCode runningKey = KeyCode.LeftShift;

    Rigidbody rigidbody;

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
    private bool wasRunning = false;

    [Space(10)]
    private float defaultZoom;
    public float zoomSpeed = 1f;

    [Header("Camera Objects")]
    public Transform cameraHolder; // Пустышка для камеры
    public Camera playerCamera;

    private Vector3 initialHolderLocalPos;
    private Quaternion initialHolderLocalRot;

    private void Start()
    {
        defaultZoom = playerCamera.fieldOfView;

        if (!IsOwner)
        {
            playerCamera.enabled = false;
            return;
        }

        rigidbody = GetComponent<Rigidbody>();
        initialCameraLocalPos = playerCamera.transform.localPosition;
        initialHolderLocalPos = cameraHolder.localPosition;
        initialHolderLocalRot = cameraHolder.localRotation;
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleZoom();
        HandleAnimations();
        HandleCameraBobbing();
        HandleCameraTilt();
        HandleCameraShake();
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        IsRunning = canRun && Input.GetKey(runningKey);

        // Если только что начали или резко прекратили бег - включаем shake
        if (IsRunning && !wasRunning || !IsRunning && wasRunning)
        {
            shakeTimer = shakeDuration;
        }
        wasRunning = IsRunning;

        float targetMovingSpeed = IsRunning ? runSpeed : speed;

        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);
        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);
    }

    void HandleZoom()
    {
        bool movingFast = Input.GetKey(runningKey) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));
        float targetZoom = movingFast ? defaultZoom + 15f : defaultZoom;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetZoom, zoomSpeed * Time.deltaTime); // Плавный зум
    }

    void HandleAnimations()
    {
        bool moving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        bool running = Input.GetKey(runningKey);

        animator.SetBool("IsWalk", moving && !running);
        animator.SetBool("IsRun", moving && running);
        animator.SetBool("IsIdle", !moving);
    }

    void HandleCameraBobbing()
    {
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        if (isMoving)
        {
            float bobSpeed = IsRunning ? runBobSpeed : walkBobSpeed;
            float bobAmount = IsRunning ? runBobAmount : walkBobAmount;

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
}
