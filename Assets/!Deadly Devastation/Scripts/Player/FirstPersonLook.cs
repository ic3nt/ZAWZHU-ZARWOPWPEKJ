using UnityEngine;
using Unity.Netcode;

public class FirstPersonLook : NetworkBehaviour
{
    [SerializeField] Transform character;
    public GameObject Head;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    private Vector2 velocity;
    private Vector2 frameVelocity;

    [Header("Camera Bobbing")]
    public float walkBobSpeed = 8f;
    public float walkBobAmount = 0.05f;
    public float runBobSpeed = 13f;
    public float runBobAmount = 0.1f;
    public float idleBobSpeed = 2f;
    public float idleBobAmount = 0.02f;

    private Vector3 initialCameraPosition;
    private float bobTimer;

    private FirstPersonMovement controller;

    void Reset()
    {
        if (!IsOwner) return;
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (IsOwner)
        {
            Head.SetActive(false);
        }

        controller = GetComponentInParent<FirstPersonMovement>();
        initialCameraPosition = transform.localPosition;
    }

    void Update()
    {
        if (!IsOwner) return;

        HandleLook();
        HandleCameraBob();
    }

    void HandleLook()
    {
        PlayerPrefs.SetFloat("currentSensitivity", sensitivity);

        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -70, 80);

        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }

    void HandleCameraBob()
    {
        float speed = 0f;
        float amount = 0f;
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        if (isMoving && controller.IsRunning)
        {
            speed = runBobSpeed;
            amount = runBobAmount;
        }
        else if (isMoving)
        {
            speed = walkBobSpeed;
            amount = walkBobAmount;
        }
        else
        {
            speed = idleBobSpeed;
            amount = idleBobAmount;
        }

        if (isMoving || !controller.IsRunning)
        {
            bobTimer += Time.deltaTime * speed;
            Vector3 bobOffset = new Vector3(0, Mathf.Sin(bobTimer) * amount, 0);
            transform.localPosition = initialCameraPosition + bobOffset;
        }
        else
        {
            // Reset position when standing still
            bobTimer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialCameraPosition, Time.deltaTime * 5f);
        }
    }
}
