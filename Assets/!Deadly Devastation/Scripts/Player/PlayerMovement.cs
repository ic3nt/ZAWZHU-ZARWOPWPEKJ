using System;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(PlayerContext))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Speed (m/s)")]
    [SerializeField] private float walkStartSpeed = 2f;
    public float walkMaxSpeed = 5f;
    [SerializeField] private float runStartSpeed = 3f;
    [SerializeField] private float runMaxSpeed = 7f;

    [Header("Dynamics (s)")]
    [SerializeField] private float accelerationTime = 0.4f;
    [SerializeField] private float decelerationTime = 0.35f;

    [Header("Run build-up")]
    [SerializeField] private float runBuildUpTime = 2.0f;
    [SerializeField] private float inertiaMultiplier = 3.0f;
    [SerializeField] private float hardStopSpeedThreshold = 3.0f;

    [Header("Falling behaviour")]
    [SerializeField] private float fallGravityMultiplier = 2.5f;

    [Header("Air control")]
    [SerializeField] private float airControlFactor = 0.4f;
    [SerializeField] private float airInertia = 0.9f;
    [SerializeField] private float minFallMoveSpeed = 1.5f;

    [Header("Falling shake")]
    [SerializeField] private float fallShakeDelay = 0.3f;
    [SerializeField] private float fallShakeInterval = 0.25f;

    [Header("Input")]
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
    [SerializeField] private bool canRun = true;

    [Header("Animation Speed Multiplier")]
    [SerializeField] private float minMultiplier = 0.8f;
    [SerializeField] private float maxMultiplier = 1.5f;
    [SerializeField] private float animSpeedSmoothTime = 0.1f;

    [Header("Turn Animations")]
    [SerializeField] private float turnSpeedThresholdDegPerSec = 30f;
    [SerializeField] private float turnSmoothTime = 0.12f;

    public bool CanMove { get; set; } = true;
    public bool IsRunning { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsFalling { get; private set; }
    public float RunProgress01 { get; private set; }
    public float RunProgress02 { get; private set; }
    public float CurrentHorizontalSpeed => new Vector3(_rb.velocity.x, 0f, _rb.velocity.z).magnitude;

    public event Action OnHardStop;
    public event Action OnSprintCollision;
    public event Action OnJumpLand;
    public event Action OnFallingShake;
    public event Action FadeOutShake;

    private PlayerContext _ctx;
    private Animator _animator;
    private Rigidbody _rb;
    private float _currentSpeed;
    private Vector3 _velSmoothRef;
    private bool _wasInputMoving;
    private bool _wasGrounded;
    private float _fallTimer;
    private float _fallShakeTimer;
    private float _multiplierVelocity;

    private Vector3 _lastForward;
    private float _turnSpeedSmooth;
    private float _turnSmoothVel;

    private static readonly int HashIdle = Animator.StringToHash("IsIdle");
    private static readonly int HashWalk = Animator.StringToHash("IsWalk");
    private static readonly int HashRun = Animator.StringToHash("IsRun");
    private static readonly int HashDance1 = Animator.StringToHash("IsDanceOne");
    private static readonly int HashMultiplier = Animator.StringToHash("Multiplier");
    private static readonly int HashTurnLeft = Animator.StringToHash("IsTurnLeft");
    private static readonly int HashTurnRight = Animator.StringToHash("IsTurnRight");

    private void Awake()
    {
        _ctx = GetComponent<PlayerContext>();
        _rb = _ctx.Rb;
        _animator = _ctx.Animator;
        _lastForward = transform.forward;
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector3 currentForward = transform.forward;
        float signedAngleThisFrame = Vector3.SignedAngle(_lastForward, currentForward, Vector3.up);
        _lastForward = currentForward;

        float degPerSec = signedAngleThisFrame / Mathf.Max(Time.deltaTime, 1e-6f);
        _turnSpeedSmooth = Mathf.SmoothDamp(_turnSpeedSmooth, degPerSec, ref _turnSmoothVel, turnSmoothTime);

        if (CanMove)
            UpdateAnimation();
        else
            ForceIdle();

        const float rayLength = 1.0f;
        bool grounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, rayLength);

        if (!grounded && _rb.velocity.y < -0.1f)
        {
            if (!IsFalling)
            {
                IsFalling = true;
                _fallTimer = 0f;
                _fallShakeTimer = 0f;
            }

            _fallTimer += Time.deltaTime;
            if (_fallTimer >= fallShakeDelay)
            {
                _fallShakeTimer += Time.deltaTime;
                if (_fallShakeTimer >= fallShakeInterval)
                {
                    OnFallingShake?.Invoke();
                    _fallShakeTimer = 0f;
                }
            }
        }

        if (!_wasGrounded && grounded)
        {
            OnJumpLand?.Invoke();

            if (IsFalling)
                FadeOutShake?.Invoke();

            _fallTimer = 0f;
            _fallShakeTimer = 0f;
            IsFalling = false;
        }

        _wasGrounded = grounded;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (CanMove)
        {
            if (IsFalling)
                ApplyFallingPhysics();
            else
                ApplyMovement();
        }
        else
        {
            HaltHorizontal();
        }
    }

    private bool HasWallAhead()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out RaycastHit hit, _ctx.WallCheckDistance))
        {
            if (((1 << hit.collider.gameObject.layer) & _ctx.WallLayer) != 0)
                return true;
        }
        return false;
    }

    private void UpdateAnimation()
    {
        if (!_animator) return;

        float speed = CurrentHorizontalSpeed;
        bool moving = speed > 0.1f;
        bool running = speed > (walkMaxSpeed + runMaxSpeed) / 2f;

        IsMoving = moving;
        IsRunning = running;

        _animator.SetBool(HashIdle, !moving);
        _animator.SetBool(HashWalk, moving && !running);
        _animator.SetBool(HashRun, running);

        float normalizedSpeed = speed / runMaxSpeed;
        float targetMultiplier = Mathf.Lerp(minMultiplier, maxMultiplier, normalizedSpeed);
        float smoothMultiplier = Mathf.SmoothDamp(
            _animator.GetFloat(HashMultiplier),
            targetMultiplier,
            ref _multiplierVelocity,
            animSpeedSmoothTime
        );
        _animator.SetFloat(HashMultiplier, smoothMultiplier);

        if (Input.GetKey(KeyCode.Alpha1))
        {
            _animator.SetBool(HashDance1, true);
            _animator.SetFloat(HashMultiplier, 1f);
        }
        else
        {
            _animator.SetBool(HashDance1, false);
        }

        if (!IsMoving && Mathf.Abs(_turnSpeedSmooth) >= turnSpeedThresholdDegPerSec)
        {
            bool turningRight = _turnSpeedSmooth > 0f;
            _animator.SetBool(HashTurnLeft, !turningRight);
            _animator.SetBool(HashTurnRight, turningRight);
        }
        else
        {
            _animator.SetBool(HashTurnLeft, false);
            _animator.SetBool(HashTurnRight, false);
        }
    }

    private void ApplyMovement()
    {
        Vector2 input = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 dir = new Vector3(input.x, 0f, input.y);
        IsMoving = dir.sqrMagnitude > 0.01f;
        if (IsMoving) dir.Normalize();

        if (_wasInputMoving && !IsMoving && CurrentHorizontalSpeed > hardStopSpeedThreshold)
            OnHardStop?.Invoke();
        _wasInputMoving = IsMoving;

        bool shift = Input.GetKey(runKey);
        IsRunning = canRun && shift && IsMoving && input.y > 0.1f && !HasWallAhead();

        if (IsRunning && IsMoving)
            RunProgress01 = Mathf.MoveTowards(RunProgress01, 1f, Time.fixedDeltaTime / Mathf.Max(0.01f, runBuildUpTime));
        else
            RunProgress01 = Mathf.MoveTowards(RunProgress01, 0f, Time.fixedDeltaTime / Mathf.Max(0.01f, runBuildUpTime * 0.75f));

        float targetMax = IsRunning && IsMoving
            ? Mathf.Lerp(runStartSpeed, runMaxSpeed, RunProgress01)
            : (IsMoving ? walkMaxSpeed : 0f);

        Vector3 worldDir = transform.TransformDirection(dir);
        float accelRate = targetMax / Mathf.Max(0.01f, accelerationTime);
        float decelRate = targetMax / Mathf.Max(0.01f, decelerationTime * Mathf.Max(1f, inertiaMultiplier));

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetMax, Time.fixedDeltaTime * (IsMoving ? accelRate : decelRate));

        Vector3 targetVel = worldDir * _currentSpeed;
        Vector3 horizVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        Vector3 smooth = Vector3.SmoothDamp(horizVel, targetVel, ref _velSmoothRef, IsMoving ? accelerationTime : decelerationTime * Mathf.Max(1f, inertiaMultiplier));

        smooth.y = _rb.velocity.y;
        _rb.velocity = smooth;

        RunProgress02 = (IsRunning && IsMoving && CurrentHorizontalSpeed >= runMaxSpeed - 5f) ? 1f : 0f;
    }

    private void ApplyFallingPhysics()
    {
        Vector2 input = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 inputDir = new Vector3(input.x, 0f, input.y);
        if (inputDir.sqrMagnitude > 0.01f) inputDir.Normalize();

        Vector3 airDir = transform.TransformDirection(inputDir);
        Vector3 horizVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        horizVel = horizVel * airInertia + airDir * airControlFactor;

        float horizontalSpeed = horizVel.magnitude;
        if (horizontalSpeed < minFallMoveSpeed && horizontalSpeed > 0.1f)
            horizVel = horizVel.normalized * minFallMoveSpeed;

        float gravityBoost = Physics.gravity.y * (fallGravityMultiplier - 1f);
        Vector3 newVel = horizVel + new Vector3(0, _rb.velocity.y + gravityBoost * Time.fixedDeltaTime, 0);
        _rb.velocity = newVel;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (IsRunning && CurrentHorizontalSpeed > 3f && col.contacts.Length > 0)
            OnSprintCollision?.Invoke();
    }

    public void AddImpulse(Vector3 impulse)
    {
        _rb.velocity += impulse;
    }

    private void HaltHorizontal()
    {
        Vector3 v = _rb.velocity;
        v.x = 0;
        v.z = 0;
        _rb.velocity = v;

        _currentSpeed = 0f;
        _velSmoothRef = Vector3.zero;
        IsRunning = false;
        IsMoving = false;
        RunProgress01 = 0f;
        _wasInputMoving = false;

        ForceIdle();
    }

    private void ForceIdle()
    {
        if (!_animator) return;
        _animator.SetBool(HashIdle, true);
        _animator.SetBool(HashRun, false);
        _animator.SetBool(HashWalk, false);
        _animator.SetFloat(HashMultiplier, 1f);
        _animator.SetBool(HashTurnLeft, false);
        _animator.SetBool(HashTurnRight, false);
    }
}
