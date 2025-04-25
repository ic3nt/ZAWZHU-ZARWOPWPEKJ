using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DragRigidbody : MonoBehaviour
{
    [Header("Capture options")]
    public float force = 1000;
    public float damping = 100;
    public float distance = 3f;
    public float minDistance = 1f;
    public float maxDistance = 10f;
    public float distanceStep = 0.5f;

    [Header("Rotate an object")]
    public float rotationSpeed = 150f;

    private Transform jointTrans;
    private float dragDepth;
    public static GameObject grabbedObject;
    private bool isDragging = false;

    void Update()
    {
        // Перетаскивание при удержании клавиши E
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryBeginDrag();
        }

        if (Input.GetKey(KeyCode.E) && isDragging)
        {
            HandleDrag(Input.mousePosition);
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            EndDrag();
        }

        HandleDistanceChange();
        HandleRotation();
    }

    void TryBeginDrag()
    {
        // Попробуем захватить объект при нажатии E
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactive"))
            {
                jointTrans = AttachJoint(hit.rigidbody, hit.point);
                grabbedObject = hit.transform.gameObject;
                isDragging = true;

                ApplyOutline(grabbedObject, true);
            }
        }
    }

    void HandleDrag(Vector3 screenPosition)
    {
        if (jointTrans == null)
            return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, distance));
        jointTrans.position = worldPos;
    }

    void EndDrag()
    {
        if (jointTrans != null)
        {
            ApplyOutline(grabbedObject, false);
            grabbedObject = null;
            Destroy(jointTrans.gameObject);
            isDragging = false;
        }
    }

    private void HandleDistanceChange()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            distance = Mathf.Clamp(distance + scroll * distanceStep, minDistance, maxDistance);
        }
    }

    private void HandleRotation()
    {
        if (jointTrans == null)
            return;

        if (Input.GetMouseButton(1)) // ПКМ для вращения
        {
            float rotationInput = Input.GetAxis("Mouse X");
            jointTrans.Rotate(Vector3.up, rotationInput * rotationSpeed * Time.deltaTime);
        }

        if (Input.GetMouseButton(0)) // ЛКМ для вращения
        {
            float rotationInput = Input.GetAxis("Mouse Y");
            jointTrans.Rotate(Vector3.left, rotationInput * rotationSpeed * Time.deltaTime);
        }
    }

    Transform AttachJoint(Rigidbody rb, Vector3 attachmentPosition)
    {
        GameObject go = new GameObject("Attachment Point");
        go.hideFlags = HideFlags.HideInHierarchy;
        go.transform.position = attachmentPosition;

        var newRb = go.AddComponent<Rigidbody>();
        newRb.isKinematic = true;

        var joint = go.AddComponent<ConfigurableJoint>();
        joint.connectedBody = rb;
        joint.configuredInWorldSpace = true;
        joint.xDrive = NewJointDrive(force, damping);
        joint.yDrive = NewJointDrive(force, damping);
        joint.zDrive = NewJointDrive(force, damping);
        joint.slerpDrive = NewJointDrive(force, damping);
        joint.rotationDriveMode = RotationDriveMode.Slerp;

        return go.transform;
    }

    private JointDrive NewJointDrive(float force, float damping)
    {
        return new JointDrive
        {
            mode = JointDriveMode.Position,
            positionSpring = force,
            positionDamper = damping,
            maximumForce = Mathf.Infinity
        };
    }

    private void ApplyOutline(GameObject obj, bool enable)
    {
        if (obj == null) return;

        OutlineScript outline = obj.GetComponent<OutlineScript>();
        if (enable)
        {
            if (!outline)
            {
                outline = obj.AddComponent<OutlineScript>();
            }
            outline.enabled = true;
        }
        else
        {
            if (outline)
            {
                outline.enabled = false;
            }
        }
    }
}
