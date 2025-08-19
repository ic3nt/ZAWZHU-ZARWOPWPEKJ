using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactMask;

    private GameObject _current;
    private Camera interactCamera;

    private void Awake()
    {
        if (!interactCamera)
        {
            var ctx = GetComponentInParent<PlayerContext>();
            if (ctx) interactCamera = ctx.Camera;
        }
    }

    private void Update()
    {
        Highlight();
        if (Input.GetMouseButtonDown(0)) TryInteract();
    }

    private void Highlight()
    {
        if (!interactCamera) return;

        Ray ray = interactCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out var hit, interactRange, interactMask) &&
            hit.collider.TryGetComponent<Interactable>(out _))
        {
            var go = hit.collider.gameObject;
            if (_current != go)
            {
                SetOutline(_current, false);
                SetOutline(go, true);
                _current = go;
            }
            return;
        }

        SetOutline(_current, false);
        _current = null;
    }

    private void TryInteract()
    {
        if (!interactCamera) return;

        Ray ray = interactCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out var hit, interactRange, interactMask) &&
            hit.collider.TryGetComponent<Interactable>(out var interactable))
        {
            interactable.Interact();
        }
    }

    private void SetOutline(GameObject obj, bool on)
    {
        if (!obj) return;

        if (!obj.TryGetComponent<OutlineScript>(out var outline) && on)
            outline = obj.AddComponent<OutlineScript>();

        if (outline) outline.enabled = on;
    }
}
