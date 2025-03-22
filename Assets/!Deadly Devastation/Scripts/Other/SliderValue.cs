using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SliderValue : MonoBehaviour
{
    public UnityEngine.UI.Slider slider1;
    public UnityEngine.UI.Slider slider2;

    private void Start()
    {
        slider1.onValueChanged.AddListener(OnSlider1ValueChanged);
        slider2.onValueChanged.AddListener(OnSlider2ValueChanged);

        if (PlayerPrefs.HasKey("SliderValue"))
        {
            float savedValue = PlayerPrefs.GetFloat("SliderValue");
            slider1.value = savedValue;
        }
    }

    private void OnSlider1ValueChanged(float value)
    {
        slider2.value = value;
    }

    private void OnSlider2ValueChanged(float value)
    {
        slider1.value = value;
    }

    public void SaveSliderValue()
    {
        float sliderValue = slider1.value;
        PlayerPrefs.SetFloat("SliderValue", sliderValue);
        PlayerPrefs.Save();
    }
}
