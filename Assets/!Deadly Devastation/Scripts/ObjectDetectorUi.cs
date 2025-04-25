using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class ObjectDetectorUI : MonoBehaviour
{
    private Camera mainCamera;
    private AudioSource audioSource;
    private Dictionary<Collider, RectTransform> activeBoxes = new();
    private HashSet<Collider> detectedObjects = new();

    [Header("General")]
    [SerializeField] private LayerMask highlightableLayer;
    [SerializeField] private LayerMask warningLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private GameObject boundingBoxPrefab;
    [SerializeField] private RectTransform playerCanvas;
    [SerializeField] private float showDuration = 5f;

    [Header("Sounds")]
    [SerializeField] private AudioClip highlightSound;
    [SerializeField] private AudioClip warningHighlightSound;

    [Header("Frame Colours")]
    [SerializeField] private Color highlightableColor = Color.yellow;
    [SerializeField] private Color warningColor = Color.red;

    private void Start()
    {
        mainCamera = Camera.main;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        DetectObjects();
        UpdateBoundingBoxes();
    }

    private void DetectObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 50f, highlightableLayer | warningLayer);
        detectedObjects.RemoveWhere(col => !System.Array.Exists(colliders, c => c == col));

        foreach (var col in colliders)
        {
            if (detectedObjects.Contains(col) || Physics.Linecast(mainCamera.transform.position, col.bounds.center, obstacleLayer))
                continue;

            detectedObjects.Add(col);
            CreateBoundingBox(col);
        }
    }

    private void CreateBoundingBox(Collider col)
    {
        if (!playerCanvas) return;

        bool isBad = ((1 << col.gameObject.layer) & warningLayer.value) != 0;
        RectTransform box = Instantiate(boundingBoxPrefab, playerCanvas).GetComponent<RectTransform>();
        box.name = $"Bounding Box UI [{col.gameObject.name}]";
        activeBoxes[col] = box;

        SetColorRecursive(box, isBad ? warningColor : highlightableColor);
        box.localScale = Vector3.zero;
        box.DOScale(Vector3.one * 1.2f, 0.3f).SetEase(Ease.OutBack);
        audioSource.PlayOneShot(isBad ? warningHighlightSound : highlightSound);

        StartCoroutine(HideAfterDelay(col));
    }

    private void UpdateBoundingBoxes()
    {
        List<Collider> toRemove = new();
        foreach (var entry in activeBoxes)
        {
            if (entry.Key)
                UpdateBoundingBoxPosition(entry.Value, entry.Key.bounds);
            else
                toRemove.Add(entry.Key);
        }
        foreach (var col in toRemove) activeBoxes.Remove(col);
    }

    private void UpdateBoundingBoxPosition(RectTransform box, Bounds bounds)
    {
        Vector3 min = new(float.MaxValue, float.MaxValue), max = new(float.MinValue, float.MinValue);
        foreach (var corner in new Vector3[]
        {
            bounds.min, bounds.max,
            new(bounds.min.x, bounds.min.y, bounds.max.z),
            new(bounds.min.x, bounds.max.y, bounds.min.z),
            new(bounds.max.x, bounds.min.y, bounds.min.z),
            new(bounds.max.x, bounds.max.y, bounds.max.z)
        })
        {
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(corner);
            if (screenPoint.z > 0)
            {
                min = Vector3.Min(min, screenPoint);
                max = Vector3.Max(max, screenPoint);
            }
        }

        if (max.x < 0 || max.y < 0 || min.x > Screen.width || min.y > Screen.height)
        {
            HideBoundingBox(GetColliderByBox(box));
            return;
        }

        Vector2 minScreenPos, maxScreenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(playerCanvas, min, mainCamera, out minScreenPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(playerCanvas, max, mainCamera, out maxScreenPos);

        box.anchoredPosition = (minScreenPos + maxScreenPos) / 2;
        box.sizeDelta = maxScreenPos - minScreenPos + new Vector2(20, 20);
    }

    private IEnumerator HideAfterDelay(Collider col)
    {
        yield return new WaitForSeconds(showDuration);
        HideBoundingBox(col);
    }

    private void HideBoundingBox(Collider col)
    {
        if (!col || !activeBoxes.TryGetValue(col, out RectTransform box)) return;

        box.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() => Destroy(box.gameObject));
        activeBoxes.Remove(col);
    }

    private void SetColorRecursive(Transform parent, Color newColor)
    {
        if (parent.TryGetComponent(out Image img))
            img.color = new Color(newColor.r, newColor.g, newColor.b, img.color.a);

        foreach (Transform child in parent)
            SetColorRecursive(child, newColor);
    }

    private Collider GetColliderByBox(RectTransform box)
    {
        foreach (var kvp in activeBoxes)
            if (kvp.Value == box) return kvp.Key;
        return null;
    }
}