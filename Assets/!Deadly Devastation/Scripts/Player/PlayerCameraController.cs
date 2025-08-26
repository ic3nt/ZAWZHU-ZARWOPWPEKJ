using UnityEngine;
using Unity.Netcode;
using FirstGearGames.SmoothCameraShaker;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Bindings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform characterRoot;
    [SerializeField] private GameObject headModel;
    [SerializeField] private PlayerMovement movement;

    [Header("Look")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float smoothing = 1.5f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-70, 80);

    [Header("Head Bobbing")]
    [SerializeField] private bool cameraBobbing = true;
    [SerializeField] private float walkBobSpeed = 8f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float runBobSpeed = 12f;
    [SerializeField] private float runBobAmount = 0.1f;
    [SerializeField] private float bobGrowthFactor = 1.6f;

    [Header("Tilt")]
    [SerializeField] private bool cameraTilt = true;
    [SerializeField] private float tiltAngle = 5f;
    [SerializeField] private float tiltSpeed = 5f;

    [Header("FOV")]
    [SerializeField] private float sprintFovBoost = 15f;
    [SerializeField] private float fovLerpSpeed = 3f;
    [SerializeField] private float fovBreathAmplitude = 1.25f;
    [SerializeField] private float fovBreathSpeed = 2.5f;

    [Header("Shake Data")]
    [SerializeField] private ShakeData hardStopShake;
    [SerializeField] private ShakeData sprintCollisionShake;
    [SerializeField] private ShakeData jumpLandShake;


    private PlayerContext _ctx;
    private Vector3 _initialHolderPos;
    private Vector3 _initialCamLocalPos;
    private float _defaultFov;
    private Vector2 _vel;
    private Vector2 _frameVel;
    private float _bobTimer;

    private void Awake()
    {
        _ctx = GetComponentInParent<PlayerContext>();
        if (!playerCamera && _ctx) playerCamera = _ctx.Camera;
        if (!movement && _ctx) movement = _ctx.Movement;
    }

    private void OnEnable()
    {
        if (movement != null)
        {
            movement.OnHardStop += OnHardStop;
            movement.OnSprintCollision += OnSprintCollision;
            movement.OnJumpLand += OnJumpLand;
        }
    }

    private void OnDisable()
    {
        if (movement != null)
        {
            movement.OnHardStop -= OnHardStop;
            movement.OnSprintCollision -= OnSprintCollision;
            movement.OnJumpLand -= OnJumpLand;
        }
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

        _initialHolderPos = cameraHolder ? cameraHolder.localPosition : Vector3.zero;
        _initialCamLocalPos = playerCamera.transform.localPosition;
        _defaultFov = playerCamera.fieldOfView;
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleLook();
        HandleFOV();
        if (cameraBobbing) HandleBobbing();
        if (cameraTilt) HandleTilt();

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

    private void HandleFOV()
    {
        float progress = movement ? movement.RunProgress01 : 0f;
        float targetFov = _defaultFov + sprintFovBoost * progress;

        float breath01 = Mathf.InverseLerp(0.6f, 1f, progress);
        if (breath01 > 0f)
            targetFov += Mathf.Sin(Time.time * fovBreathSpeed) * fovBreathAmplitude * breath01;

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov, fovLerpSpeed * Time.deltaTime);
    }

    private void HandleBobbing()
    {
        if (!movement) return;

        if (movement.IsMoving)
        {
            float spd = movement.IsRunning ? runBobSpeed : walkBobSpeed;
            float amt = movement.IsRunning ? runBobAmount : walkBobAmount;
            float growth = Mathf.Lerp(1f, bobGrowthFactor, movement.RunProgress01);
            amt *= growth;

            _bobTimer += Time.deltaTime * spd;
            Vector3 offset = new Vector3(0, Mathf.Sin(_bobTimer) * amt, 0);
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                _initialCamLocalPos + offset,
                Time.deltaTime * 10f
            );
        }
        else
        {
            _bobTimer = 0f;
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                _initialCamLocalPos,
                Time.deltaTime * 10f
            );
        }
    }

    private void HandleTilt()
    {
        if (!cameraHolder) return;

        float target = 0f;
        if (Input.GetKey(KeyCode.A)) target = tiltAngle;
        else if (Input.GetKey(KeyCode.D)) target = -tiltAngle;

        float currentZ = cameraHolder.localRotation.eulerAngles.z;
        float smoothZ = Mathf.LerpAngle(currentZ, target, Time.deltaTime * tiltSpeed);
        cameraHolder.localRotation = Quaternion.Euler(0, 0, smoothZ);
    }

    private void OnHardStop()
    {
        if (hardStopShake != null) CameraShakerHandler.Shake(hardStopShake);
    }

    private void OnSprintCollision()
    {
        if (sprintCollisionShake != null) CameraShakerHandler.Shake(sprintCollisionShake);
    }

    private void OnJumpLand()
    {
        if (jumpLandShake != null) CameraShakerHandler.Shake(jumpLandShake);
    }
}
