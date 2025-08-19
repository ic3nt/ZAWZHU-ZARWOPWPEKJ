using UnityEngine;
using Unity.Netcode;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Bindings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private bool useCameraForHUD = false;
    [SerializeField] private Camera hudCamera;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform characterRoot;
    [SerializeField] private GameObject headModel;
    [SerializeField] private PlayerMovement movement;

    [Header("Look")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float smoothing = 1.5f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-70, 80);

    [Header("Bobbing")]
    [SerializeField] private bool cameraBobbing = true;
    [SerializeField] private float walkBobSpeed = 8f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float runBobSpeed = 12f;
    [SerializeField] private float runBobAmount = 0.1f;

    [Header("Tilt")]
    [SerializeField] private bool cameraTilt = true;
    [SerializeField] private float tiltAngle = 5f;
    [SerializeField] private float tiltSpeed = 5f;

    [Header("Shake")]
    [SerializeField] private float shakeIntensity = 0.05f;
    [SerializeField] private float shakeDuration = 0.2f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float zoomFOV = 30f;

    [Header("Enemy Focus")]
    [SerializeField] private bool focusOnEnemy = true;
    [SerializeField] private float focusRayDistance = 100f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip enemyDetectedSound;
    [SerializeField] private bool shakeOnFocus = true;
    [SerializeField] private float focusShakeIntensity = 0.1f;

    private PlayerContext _ctx;
    private Vector3 _initialHolderPos;
    private Vector3 _initialCamLocalPos;
    private float _defaultFov;
    private Vector2 _vel;          // accumulated
    private Vector2 _frameVel;     // smoothed frame delta
    private float _bobTimer;
    private float _shakeTimer;
    private bool _zoomed;
    private Transform _focusTarget;

    private void Awake()
    {
        _ctx = GetComponentInParent<PlayerContext>();
        if (!playerCamera) playerCamera = _ctx.Camera;
        if (!movement) movement = _ctx.Movement;
    }

    private void Start()
    {
        if (!IsOwner)
        {
            if (playerCamera) playerCamera.enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;

        if (headModel) headModel.SetActive(false);

        _initialHolderPos = cameraHolder.localPosition;
        _initialCamLocalPos = playerCamera.transform.localPosition;
        _defaultFov = playerCamera.fieldOfView;

        if (useCameraForHUD && hudCamera)
            SyncHUDCamera();
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleZoom();
        HandleShake();
        HandleLook();
        if (cameraBobbing) HandleBobbing();
        if (cameraTilt) HandleTilt();
        if (focusOnEnemy) HandleEnemyFocus();
        if (useCameraForHUD && hudCamera) SyncHUDCamera();
    }

    private void SyncHUDCamera()
    {
        hudCamera.fieldOfView = playerCamera.fieldOfView;
        hudCamera.transform.SetPositionAndRotation(playerCamera.transform.position, playerCamera.transform.rotation);
    }

    private void HandleZoom()
    {
        float targetFov = _zoomed ? zoomFOV : _defaultFov;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov, zoomSpeed * Time.deltaTime);
    }

    private void HandleBobbing()
    {
        if (!movement) return;

        if (movement.IsMoving)
        {
            float spd = movement.IsRunning ? runBobSpeed : walkBobSpeed;
            float amt = movement.IsRunning ? runBobAmount : walkBobAmount;
            _bobTimer += Time.deltaTime * spd;
            Vector3 offset = new Vector3(0, Mathf.Sin(_bobTimer) * amt, 0);
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, _initialCamLocalPos + offset, Time.deltaTime * 10f);
        }
        else
        {
            _bobTimer = 0f;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, _initialCamLocalPos, Time.deltaTime * 10f);
        }
    }

    private void HandleTilt()
    {
        float target = 0f;
        if (Input.GetKey(KeyCode.A)) target = tiltAngle;
        else if (Input.GetKey(KeyCode.D)) target = -tiltAngle;

        float currentZ = cameraHolder.localRotation.eulerAngles.z;
        float smoothZ = Mathf.LerpAngle(currentZ, target, Time.deltaTime * tiltSpeed);
        cameraHolder.localRotation = Quaternion.Euler(0, 0, smoothZ);
    }

    private void HandleShake()
    {
        if (_shakeTimer > 0 && movement && movement.IsRunning)
        {
            Vector3 off = new(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0);
            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, _initialHolderPos + off, Time.deltaTime * 15f);
            _shakeTimer -= Time.deltaTime;
        }
        else
        {
            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, _initialHolderPos, Time.deltaTime * 5f);
        }

        if (shakeOnFocus && _focusTarget)
        {
            Vector3 off = new(
                Random.Range(-focusShakeIntensity, focusShakeIntensity),
                Random.Range(-focusShakeIntensity, focusShakeIntensity),
                0);
            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, _initialHolderPos + off, Time.deltaTime * 20f);
        }
    }

    private void HandleLook()
    {
        Vector2 md = new(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 raw = md * sensitivity;
        _frameVel = Vector2.Lerp(_frameVel, raw, 1f / Mathf.Max(0.0001f, smoothing));
        _vel += _frameVel;
        _vel.y = Mathf.Clamp(_vel.y, pitchLimits.x, pitchLimits.y);

        transform.localRotation = Quaternion.AngleAxis(-_vel.y, Vector3.right);
        characterRoot.localRotation = Quaternion.AngleAxis(_vel.x, Vector3.up);
    }

    public void TriggerShake(float intensity, float duration)
    {
        shakeIntensity = intensity;
        _shakeTimer = duration;
    }

    private void HandleEnemyFocus()
    {
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, focusRayDistance, enemyLayer))
        {
            _zoomed = true;
            _focusTarget = hit.transform;
            if (audioSource && enemyDetectedSound && !audioSource.isPlaying)
                audioSource.PlayOneShot(enemyDetectedSound);
        }
        else
        {
            _zoomed = false;
            _focusTarget = null;
        }
    }
}