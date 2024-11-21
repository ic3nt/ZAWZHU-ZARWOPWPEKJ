using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    public bool canSway;
    public Vector3 inSwP;
    public float swayAm;
    public float maxswayAm;
    public float swaySm;

    
    private void LateUpdate()
    {
        if (canSway)
        {
            float mX = -Input.GetAxis("Mouse X") * swayAm;
            float mY = -Input.GetAxis("Mouse Y") * swayAm;

            mX = Mathf.Clamp(mX, -maxswayAm, maxswayAm);
            mY = Mathf.Clamp(mY, -maxswayAm, maxswayAm);
            Vector3 finalSwayPos = new Vector3(mX, mY, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalSwayPos + inSwP, Time.deltaTime * swaySm);

        }
    }

    
}
