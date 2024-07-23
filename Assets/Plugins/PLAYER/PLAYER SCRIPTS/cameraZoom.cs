using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraZoom : MonoBehaviour
{
    private float defaultZoom;
    private float maxZoom;
    public float zoomSpeed = 1f;
    public float returnSpeed = 1f;
    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        defaultZoom = camera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire2") && camera.fieldOfView > 12)
        {

                ZoomCamera(defaultZoom - 10f);
  
            
        }
        else
        {
            ZoomCamera(defaultZoom);
        }

    }

    private void ZoomCamera(float targetZoom)
    {
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetZoom, zoomSpeed * Time.deltaTime);
    }


}
