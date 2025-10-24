using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DragRigidbody : MonoBehaviour
{
    public float force = 600;
    public float damping = 6;
    public float distance = 15;

    public Camera dragCamera; //  амера, с которой идет взаимодействие (например, камера с RenderTexture)
    public static GameObject grabbedObject;

    private Transform jointTrans;
    private Rigidbody attachedRb;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleInputBegin();
        }
        else if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HandleInputEnd();
        }
    }

    private void Awake()
    {
        dragCamera = Camera.main;
    }

    void HandleInputBegin()
    {
        if (dragCamera == null)
        {
   //         Debug.LogWarning("Drag camera not assigned.");
            return;
        }

        Ray ray = dragCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactive"))
            {
                grabbedObject = hit.collider.gameObject;
                jointTrans = AttachJoint(hit.rigidbody, hit.point);
                attachedRb = hit.rigidbody;
            }
        }
    }

    void HandleInput()
    {
        if (jointTrans == null || dragCamera == null)
            return;

        // Ќаправление Ч пр€мо перед камерой
        Ray ray = dragCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        jointTrans.position = ray.origin + ray.direction * distance;
    }

    void HandleInputEnd()
    {
        if (jointTrans != null)
        {
            Destroy(jointTrans.gameObject);
            jointTrans = null;
        }

        grabbedObject = null;
        attachedRb = null;
    }

    Transform AttachJoint(Rigidbody rb, Vector3 attachmentPosition)
    {
        GameObject go = new GameObject("Attachment Point");
        go.hideFlags = HideFlags.HideInHierarchy;
        go.transform.position = attachmentPosition;

        Rigidbody newRb = go.AddComponent<Rigidbody>();
        newRb.isKinematic = true;

        ConfigurableJoint joint = go.AddComponent<ConfigurableJoint>();
        joint.connectedBody = rb;
        joint.configuredInWorldSpace = true;

        joint.xDrive = NewJointDrive(force, damping);
        joint.yDrive = NewJointDrive(force, damping);
        joint.zDrive = NewJointDrive(force, damping);
        joint.slerpDrive = NewJointDrive(force, damping);
        joint.rotationDriveMode = RotationDriveMode.Slerp;

        return go.transform;
    }

    JointDrive NewJointDrive(float force, float damping)
    {
        return new JointDrive
        {
            mode = JointDriveMode.Position,
            positionSpring = force,
            positionDamper = damping,
            maximumForce = Mathf.Infinity
        };
    }
}
