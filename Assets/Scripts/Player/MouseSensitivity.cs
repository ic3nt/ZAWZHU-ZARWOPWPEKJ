using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
    public Slider slider;
    public FirstPersonLook FirstPersonLook;
    public Transform playerBody;
    float xRotation = 0f;
    float yRotation = 0f;

    void Start()
    {
        FirstPersonLook.sensitivity = PlayerPrefs.GetFloat("currentSensitivity", 100);
        slider.value = FirstPersonLook.sensitivity = 2;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void AdjustSpeed(float newSpeed)
    {
        FirstPersonLook.sensitivity = newSpeed;
    }
}