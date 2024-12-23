using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class FirstPersonMovement : NetworkBehaviour
{
    public float speed = 5;

    public Animator animator;

    public Camera camera;
    private float defaultZoom;
    public float zoomSpeed = 1f;
    public float returnSpeed = 1f;

    [Header("Running")]
    public bool canRun = true;

    public bool IsRunning { get; private set; }
    public float runSpeed = 7;
    public KeyCode runningKey = KeyCode.LeftShift;

    Rigidbody rigidbody;

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    private void Start()
    {
        defaultZoom = camera.fieldOfView;

        if (!IsOwner)
        {
            camera.enabled = false;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        if ((Input.GetKey(KeyCode.LeftShift)) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            ZoomCamera(defaultZoom + 15f);
        }
        else
        {
            ZoomCamera(defaultZoom);
        }



        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))

        {
            animator.SetBool("IsDanceOne", false);

            animator.SetBool("IsWalk", true);
            animator.SetBool("IsRun", false);
            animator.SetBool("IsIdle", false);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("IsRun", true);
                animator.SetBool("IsWalk", false);
                animator.SetBool("IsIdle", false);
            }
        }
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsRun", false);
            animator.SetBool("IsWalk", false);
            if (Input.GetKey(KeyCode.Alpha1))
            {
                animator.SetBool("IsDanceOne", true);
            }
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            animator.SetBool("IsWalk", true);
            animator.SetBool("IsRun", false);
            animator.SetBool("IsIdle", false);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("IsRun", true);
                animator.SetBool("IsWalk", false);
                animator.SetBool("IsIdle", false);
            }
        }
    }
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        IsRunning = canRun && Input.GetKey(KeyCode.LeftShift);

        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);

    }
    public void ZoomCamera(float targetZoom)
    {
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetZoom, zoomSpeed * Time.deltaTime);
    }
}
