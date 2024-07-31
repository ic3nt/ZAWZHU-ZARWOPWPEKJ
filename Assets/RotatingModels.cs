using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingModels : MonoBehaviour
{
    public float rotatingSpeed = 10f;
    public float deceleration = 0.95f;
    private bool isRotating = false;
    private float startMousePosition;
    private float currentRotationX;
    private float currentRotationSpeed;

    void Update()
    {
        if (isRotating)
        {
            float currentMousePosition = Input.mousePosition.x;
            float mouseMovement = currentMousePosition - startMousePosition;
            startMousePosition = currentMousePosition;

            currentRotationSpeed = mouseMovement * rotatingSpeed * Time.deltaTime;
            currentRotationX -= currentRotationSpeed;

            RotateModel();
        }
        else if (currentRotationSpeed != 0)
        {
            currentRotationSpeed *= deceleration;

            if (Mathf.Abs(currentRotationSpeed) < 0.01f)
            {
                currentRotationSpeed = 0;
            }

            currentRotationX -= currentRotationSpeed;

            RotateModel();
        }

        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("A");
            currentRotationSpeed -= rotatingSpeed * Time.deltaTime;
            currentRotationX -= currentRotationSpeed * Time.deltaTime;

            RotateModel();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("D");
            currentRotationSpeed += rotatingSpeed * Time.deltaTime;
            currentRotationX += currentRotationSpeed * Time.deltaTime;

            RotateModel();
        }
    }

    public void OnMouseEnter()
    {
        Debug.Log("Mouse Entered");
    }

    public void OnMouseExit()
    {
        Debug.Log("Mouse Exited");
        isRotating = false;
        currentRotationSpeed *= deceleration;
    }

    void OnMouseDown()
    {
        isRotating = true;
        startMousePosition = Input.mousePosition.x;
        currentRotationSpeed = 0;
    }

    void OnMouseUp()
    {
        isRotating = false;
    }

    private void RotateModel()
    {
        Quaternion targetRotation = Quaternion.Euler(0, currentRotationX, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }
}
