using UnityEngine;
using RKS.DD.Core;

namespace RKS.DD.Menu
{
    public class RotateModelByInput : RKSBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private float rotatingSpeed = 200f;
        [SerializeField] private float deceleration = 4f;

        private float currentRotationSpeed = 0f;
        private float lastMouseX;
        private bool dragging = false;
        private float rotationY = 0f;

        protected override void Update()
        {
            HandleMouseDrag();
            HandleKeyboardInput();

            if (!dragging)
            {
                currentRotationSpeed = Mathf.MoveTowards(
                    currentRotationSpeed,
                    0f,
                    deceleration * Time.deltaTime
                );
            }

            rotationY += currentRotationSpeed * Time.deltaTime;

            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }

        private void HandleMouseDrag()
        {
            if (dragging)
            {
                float deltaX = Input.mousePosition.x - lastMouseX;
                lastMouseX = Input.mousePosition.x;

                currentRotationSpeed = -deltaX * rotatingSpeed * 0.01f;
            }

            if (Input.GetMouseButtonDown(0))
            {
                dragging = true;
                lastMouseX = Input.mousePosition.x;
            }

            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
            }
        }

        private void HandleKeyboardInput()
        {
            if (Input.GetKey(KeyCode.A))
            {
                currentRotationSpeed -= rotatingSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                currentRotationSpeed += rotatingSpeed * Time.deltaTime;
            }
        }
    }
}