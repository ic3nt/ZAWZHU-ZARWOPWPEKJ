using UnityEngine;

public interface IInteract
{
    void Interact();
}

public class PlayerInteractive : MonoBehaviour
{
    [Header("Interaction")]
    public Camera interactCamera;
    public float interactRange = 3f;
    public LayerMask interactMask;

    private GameObject currentHighlightedObject;

    void Update()
    {
        HighlightInteractable();

        if (Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }
    }

    void HighlightInteractable()
    {
        if (interactCamera == null) return;

        Ray ray = interactCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange, interactMask))
        {
            var interactable = hitInfo.collider.GetComponent<IInteract>();
            if (interactable != null)
            {
                GameObject hitObject = hitInfo.collider.gameObject;

                if (currentHighlightedObject != hitObject)
                {
                    RemoveOutline(currentHighlightedObject);
                    ApplyOutline(hitObject, true);
                    currentHighlightedObject = hitObject;
                }

                return;
            }
        }

        // Если луч никуда не попал или объект не интерактивный — убрать подсветку
        RemoveOutline(currentHighlightedObject);
        currentHighlightedObject = null;
    }

    void TryInteract()
    {
        if (interactCamera == null)
        {
            Debug.LogWarning("Не назначена камера для взаимодействия!", this);
            return;
        }

        Ray ray = interactCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange, interactMask))
        {
            if (hitInfo.collider.TryGetComponent<IInteract>(out var interactable))
            {
                interactable.Interact();
            }
        }
    }

    void ApplyOutline(GameObject obj, bool enable)
    {
        if (obj == null) return;

        OutlineScript outline = obj.GetComponent<OutlineScript>();
        if (enable)
        {
            if (outline == null)
                outline = obj.AddComponent<OutlineScript>();

            outline.enabled = true;
        }
        else
        {
            if (outline != null)
                outline.enabled = false;
        }
    }

    void RemoveOutline(GameObject obj)
    {
        ApplyOutline(obj, false);
    }
}
