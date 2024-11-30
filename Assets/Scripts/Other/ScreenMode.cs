using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenMode : MonoBehaviour
{
    public void FullscreenMode()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
