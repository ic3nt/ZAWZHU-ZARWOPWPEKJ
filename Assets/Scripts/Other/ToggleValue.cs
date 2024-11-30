using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleValue : MonoBehaviour
{
    public Toggle toggle1;
    public Toggle toggle2;

    void Start()
    {
        toggle1.onValueChanged.AddListener(OnToggle1ValueChanged);
        toggle2.onValueChanged.AddListener(OnToggle2ValueChanged);
    }

    private void OnToggle1ValueChanged(bool value)
    {
        toggle2.isOn = value;
    }

    private void OnToggle2ValueChanged(bool value)
    {
        toggle1.isOn = value;
    }
}
