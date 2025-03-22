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

    private bool wasRunning = false;

    private void Start()
    {
        if (!IsOwner) return;

        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        IsRunning = canRun && Input.GetKey(runningKey);

        // Если только что начали или резко прекратили бег - включаем shake
        if (IsRunning && !wasRunning || !IsRunning && wasRunning)
        {
            wasRunning = IsRunning;
        }

        float targetMovingSpeed = IsRunning ? runSpeed : speed;

        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);
        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);
    }

    void HandleAnimations()
    {
        bool moving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        bool running = Input.GetKey(runningKey);

        animator.SetBool("IsWalk", moving && !running);
        animator.SetBool("IsRun", moving && running);
        animator.SetBool("IsIdle", !moving);
    }
}
