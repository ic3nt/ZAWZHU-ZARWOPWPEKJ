using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class OutlineRender : NetworkBehaviour
{
    public float rayDistance = 3f;
    private GameObject currentObject;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactive"))
            {
                if (hit.collider.gameObject != DragRigidbody.grabbedObject)
                {
                    ApplyOutline(hit.collider.gameObject, true);
                    currentObject = hit.collider.gameObject;
                }
            }
            else
            {
                RemoveOutline(currentObject);
            }
        }
        else
        {
            RemoveOutline(currentObject);
        }

        if (DragRigidbody.grabbedObject != null)
        {
            ApplyOutline(DragRigidbody.grabbedObject, true);
        }
    }

    private void ApplyOutline(GameObject obj, bool enable)
    {
        if (obj == null) return;

        OutlineScript outline = obj.GetComponent<OutlineScript>();
        if (enable)
        {
            if (!outline)
            {
                outline = obj.AddComponent<OutlineScript>();
            }
            outline.enabled = true;
        }
    }

    private void RemoveOutline(GameObject obj)
    {
        if (obj == null) return;

        OutlineScript outline = obj.GetComponent<OutlineScript>();
        if (outline && obj != DragRigidbody.grabbedObject)
        {
            outline.enabled = false;
        }
    }
}
