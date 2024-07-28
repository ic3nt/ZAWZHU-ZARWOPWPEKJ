using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
public class FirstPersonLook : NetworkBehaviour
{

    [SerializeField]
    Transform character;
    public GameObject Head;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    Vector2 velocity;
    Vector2 frameVelocity;

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
    }
    void Update()
    {
        if (!IsOwner) return;

        PlayerPrefs.SetFloat("currentSensitivity", sensitivity);
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -70, 80);

        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);

    }
}
