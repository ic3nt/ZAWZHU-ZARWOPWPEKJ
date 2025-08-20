using System;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(PlayerContext))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Speed (m/s)")]
    [SerializeField] private float walkStartSpeed = 2f;
    [SerializeField] private float walkMaxSpeed = 5f;
    [SerializeField] private float runStartSpeed = 3f;
    [SerializeField] private float runMaxSpeed = 7f;

    [Header("Dynamics (s)")]
    [SerializeField] private float accelerationTime = 0.4f;
    [SerializeField] private float decelerationTime = 0.35f;

    [Header("Run build-up")]
    [SerializeField] private float runBuildUpTime = 2.0f;
    [SerializeField] private float inertiaMultiplier = 3.0f;
    [SerializeField] private float hardStopSpeedThreshold = 3.0f;

    [Header("Input")]
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
    [SerializeField] private bool canRun = true;

    public bool CanMove { get; set; } = true;
    public bool IsRunning { get; private set; }
    public bool IsMoving { get; private set; }
    public float RunProgress01 { get; private set; }
    public float RunProgress02 { get; private set; }
    public float CurrentHorizontalSpeed => new Vector3(_rb.velocity.x, 0f, _rb.velocity.z).magnitude;

    public event Action OnHardStop;
    public event Action OnSprintCollision;
    public event Action OnJumpLand;

    private PlayerContext _ctx;
    private Animator _animator;
    private Rigidbody _rb;

    private float _currentSpeed;
    private Vector3 _velSmoothRef;
    private bool _wasInputMoving;
    private bool _wasGrounded;

    private static readonly int HashIdle = Animator.StringToHash("IsIdle");
    private static readonly int HashWalk = Animator.StringToHash("IsWalk");
    private static readonly int HashRun = Animator.StringToHash("IsRun");
    private static readonly int HashDance1 = Animator.StringToHash("IsDanceOne");

    private void Awake()
    {
        _ctx = GetComponent<PlayerContext>();
        _rb = _ctx.Rb;
        if (!_animator) _animator = _ctx.Animator;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (CanMove) UpdateAnimation();
        else ForceIdle();

        bool grounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f);
        if (!_wasGrounded && grounded) OnJumpLand?.Invoke();
        _wasGrounded = grounded;

        Debug.Log($"[PlayerMovement] Running: {IsRunning}, Speed: {CurrentHorizontalSpeed:F2}");
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (CanMove) ApplyMovement();
        else HaltHorizontal();
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
        IsMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f;
        bool shift = Input.GetKey(runKey);

        IsRunning = canRun && shift && IsMoving && Input.GetAxisRaw("Vertical") > 0.1f && !HasWallAhead();

        if (!_animator) return;

        if (IsMoving)
        {
            _animator.SetBool(HashDance1, false);
            _animator.SetBool(HashWalk, !IsRunning);
            _animator.SetBool(HashRun, IsRunning);
            _animator.SetBool(HashIdle, false);
        }
        else
        {
            _animator.SetBool(HashIdle, true);
            _animator.SetBool(HashRun, false);
            _animator.SetBool(HashWalk, false);

            if (Input.GetKey(KeyCode.Alpha1))
                _animator.SetBool(HashDance1, true);
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

        _currentSpeed = Mathf.MoveTowards(
            _currentSpeed,
            targetMax,
            Time.fixedDeltaTime * (IsMoving ? accelRate : decelRate)
        );

        Vector3 targetVel = worldDir * _currentSpeed;
        Vector3 horizVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        float smoothTime = IsMoving ? accelerationTime : decelerationTime * Mathf.Max(1f, inertiaMultiplier);
        Vector3 smooth = Vector3.SmoothDamp(horizVel, targetVel, ref _velSmoothRef, smoothTime);

        smooth.y = _rb.velocity.y;
        _rb.velocity = smooth;

        if (IsRunning && IsMoving && CurrentHorizontalSpeed >= runMaxSpeed - 5f)
            RunProgress02 = 1f;
        else
            RunProgress02 = 0f;
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
    }
}
