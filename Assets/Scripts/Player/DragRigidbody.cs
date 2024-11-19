using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DragRigidbody : MonoBehaviour
{
    public float force = 1000;
    public float damping = 100;
    public float distance = 3;
    public float rotationSpeed = 150f;

    Transform jointTrans;
    float dragDepth;

    void OnMouseDown()
    {
        HandleInputBegin(Input.mousePosition);
    }

    public void OnMouseUp()
    {
        HandleInputEnd(Input.mousePosition);
    }

    void OnMouseDrag()
    {
        HandleInput(Input.mousePosition);
    }

    void Update()
    {
        HandleRotation();
    }

    public void HandleInputBegin(Vector3 screenPosition)
    {
        // простым языком чекаем положение камеры в мире, кидая рэйкаст, если он попадаем на объект с слоем Interactive, то мы можем брать этот объект (если что это пояснение ко всей работе скрипта, сорри мне лень пояснять все методы)

        var ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactive"))
            {
                dragDepth = CameraPlane.CameraToPointDepth(Camera.main, hit.point);
                jointTrans = AttachJoint(hit.rigidbody, hit.point);
            }
        }
    }

    public void HandleInput(Vector3 screenPosition)
    {
        if (jointTrans == null)
            return;

        var worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        jointTrans.position = CameraPlane.ScreenToWorldPlanePoint(Camera.main, dragDepth, screenPosition);
        DrawRope();
    }

    public void HandleInputEnd(Vector3 screenPosition)
    {
        Destroy(jointTrans.gameObject);
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
        JointDrive drive = new JointDrive();
        drive.mode = JointDriveMode.Position;
        drive.positionSpring = force;
        drive.positionDamper = damping;
        drive.maximumForce = Mathf.Infinity;
        return drive;
    }

    private void DrawRope()
    {
        if (jointTrans == null)
        {
            return;
        }
    }

    private void HandleRotation()
    {
        if (jointTrans == null)
            return;

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            jointTrans.Rotate(Vector3.up, scrollInput * rotationSpeed);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            jointTrans.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            jointTrans.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}
