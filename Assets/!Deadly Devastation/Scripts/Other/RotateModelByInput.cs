using UnityEngine;
using RKS.DD.Core;

namespace RKS.DD.Menu
{
    public class RotateModelByInput : RKSBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private float rotatingSpeed = 10f;
        [SerializeField] private float deceleration = 0.95f;
        [SerializeField] private float returnDelay = 5f;
        [SerializeField] private float returnSpeed = 2f;

        private bool isDragging = false;
        private bool isMouseOver = false;

        private float startMouseX;
        private float rotationSpeed;
        private float currentRotationY;

        private Quaternion initialRotation;
        private float lastActionTime;

        protected override void OnReady()
        {
            initialRotation = transform.rotation;
            currentRotationY = transform.eulerAngles.y;
            lastActionTime = Time.time;
        }

        protected override void Update()
        {
            if (isMouseOver) 
            {
                HandleMouseRotation();
                HandleKeyboardRotation();
            }

            ApplyInertia();
            ApplyRotation();
            CheckAutoReturn();
        }

        void OnMouseEnter()
        {
            isMouseOver = true;
        }

        void OnMouseExit()
        {
            isMouseOver = false;
            isDragging = false;
        }

        void HandleMouseRotation()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                startMouseX = Input.mousePosition.x;
                rotationSpeed = 0;
                lastActionTime = Time.time;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                lastActionTime = Time.time;
            }

            if (isDragging)
            {
                float curMouseX = Input.mousePosition.x;
                float delta = curMouseX - startMouseX;
                startMouseX = curMouseX;

                rotationSpeed = -delta * rotatingSpeed * 0.02f;
                currentRotationY += rotationSpeed;

                lastActionTime = Time.time;
            }
        }

        void HandleKeyboardRotation()
        {
            if (Input.GetKey(KeyCode.A))
            {
                rotationSpeed -= rotatingSpeed * Time.deltaTime;
                lastActionTime = Time.time;
            }

            if (Input.GetKey(KeyCode.D))
            {
                rotationSpeed += rotatingSpeed * Time.deltaTime;
                lastActionTime = Time.time;
            }
        }

        void ApplyInertia()
        {
            if (!isDragging)
            {
                rotationSpeed *= deceleration;
            }

            if (Mathf.Abs(rotationSpeed) < 0.001f)
                rotationSpeed = 0;

            currentRotationY += rotationSpeed;
        }

        void ApplyRotation()
        {
            Quaternion target = Quaternion.Euler(0, currentRotationY, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 10f);
        }

        void CheckAutoReturn()
        {
            if (Time.time - lastActionTime >= returnDelay && !isDragging)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    initialRotation,
                    Time.deltaTime * returnSpeed
                );

                currentRotationY = transform.eulerAngles.y;
            }
        }
    }
}
