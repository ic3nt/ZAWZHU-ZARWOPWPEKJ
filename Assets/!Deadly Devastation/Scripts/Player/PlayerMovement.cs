using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Variables")]
    public float walkStartSpeed = 2f;
    public float walkMaxSpeed = 5f;
    public float runStartSpeed = 3f;
    public float runMaxSpeed = 7f;
    public bool canRun = true;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Acceleration / Deceleration")]
    [SerializeField] private float accelerationTime = 0.5f;
    [SerializeField] private float decelerationTime = 0.3f;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody rigidbody;
    private Vector3 velocitySmoothDamp = Vector3.zero;
    private float currentSpeed = 0f;
    public bool IsRunning { get; private set; }
    [HideInInspector] public bool IsMoving = false;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        HandleAnimation();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        HandleMovement();
    }

    private void HandleAnimation()
    {
        IsMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        bool isShift = Input.GetKey(KeyCode.LeftShift);

        if (IsMoving)
        {
            animator.SetBool("IsDanceOne", false);
            animator.SetBool("IsWalk", !isShift);
            animator.SetBool("IsRun", isShift);
            animator.SetBool("IsIdle", false);
        }
        else
        {
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsRun", false);
            animator.SetBool("IsWalk", false);

            if (Input.GetKey(KeyCode.Alpha1))
            {
                animator.SetBool("IsDanceOne", true);
            }
        }
    }

    private void HandleMovement()
    {
        IsRunning = canRun && Input.GetKey(runningKey);
        float targetMaxSpeed = IsRunning ? runMaxSpeed : walkMaxSpeed;
        float startSpeed = IsRunning ? runStartSpeed : walkStartSpeed;

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 inputDir = new Vector3(input.x, 0, input.y);

        IsMoving = inputDir.magnitude > 0.1f;

        if (IsMoving)
            inputDir.Normalize();

        Vector3 worldInputDir = transform.TransformDirection(inputDir);

        if (IsMoving)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetMaxSpeed, Time.fixedDeltaTime * (targetMaxSpeed / accelerationTime));
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, Time.fixedDeltaTime * (targetMaxSpeed / decelerationTime));
        }

        Vector3 targetVelocity = worldInputDir * currentSpeed;
        Vector3 horizontalVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

        Vector3 smoothedVelocity = Vector3.SmoothDamp(horizontalVelocity, targetVelocity, ref velocitySmoothDamp, IsMoving ? accelerationTime : decelerationTime);

        smoothedVelocity.y = rigidbody.velocity.y;
        rigidbody.velocity = smoothedVelocity;
    }
}
