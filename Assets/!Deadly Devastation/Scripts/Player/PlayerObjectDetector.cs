using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class PlayerObjectDetector : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private LayerMask highlightableLayers;
    [SerializeField] private LayerMask warningLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private GameObject boundingBoxPrefab;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private float showDuration = 5f;
    [SerializeField, Tooltip("Increase the size of the frame relative to the object (1.2 = +20%)")]
    private float boxSizeMultiplier = 1.2f;
    [SerializeField] private const int maxDetectedObjects = 15;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip highlightClip;
    [SerializeField] private AudioClip warningClip;

    [Header("Box Colors")]
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Color warningColor = Color.red;

    private Camera mainCamera;
    private AudioSource audioSource;

    private readonly Dictionary<Collider, RectTransform> activeBoxes = new();
    private readonly HashSet<Collider> detectedObjects = new();
    private readonly Queue<Collider> detectionQueue = new();

    private void Awake()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        DetectObjectsInView();
        UpdateBoxes();
    }

    private void DetectObjectsInView()
    {
        Collider[] allColliders = FindObjectsOfType<Collider>();
        foreach (var col in allColliders)
        {
            if (!IsInLayerMask(col.gameObject, highlightableLayers))
                continue;

            if (detectedObjects.Contains(col))
                continue;

            if (!IsVisibleToCamera(col))
                continue;

            if (Physics.Linecast(mainCamera.transform.position, col.bounds.center, obstacleLayer))
                continue;

            detectedObjects.Add(col);
            detectionQueue.Enqueue(col);

            while (detectedObjects.Count > maxDetectedObjects)
            {
                var oldest = detectionQueue.Dequeue();
                detectedObjects.Remove(oldest);
                HideBox(oldest);
            }

            ShowBoxFor(col);
        }
    }

    private bool IsVisibleToCamera(Collider col)
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(col.bounds.center);
        return viewportPos.z > 0 &&
               viewportPos.x >= 0 && viewportPos.x <= 1 &&
               viewportPos.y >= 0 && viewportPos.y <= 1;
    }

    private void ShowBoxFor(Collider col)
    {
        if (canvasRect == null || boundingBoxPrefab == null) return;
        if (activeBoxes.ContainsKey(col)) return;

        bool isWarning = (warningLayer == (warningLayer | (1 << col.gameObject.layer)));
        Color color = isWarning ? warningColor : highlightColor;
        AudioClip clip = isWarning ? warningClip : highlightClip;

        RectTransform box = Instantiate(boundingBoxPrefab, canvasRect).GetComponent<RectTransform>();
        box.name = $"Box [{col.name}]";
        SetBoxColor(box, color);
        box.localScale = Vector3.zero;

        activeBoxes[col] = box;

        box.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        audioSource.PlayOneShot(clip);
        StartCoroutine(HideBoxAfterDelay(col, showDuration));
    }

    private void UpdateBoxes()
    {
        List<Collider> toRemove = new();

        foreach (var pair in activeBoxes)
        {
            if (pair.Key == null)
            {
                toRemove.Add(pair.Key);
                continue;
            }

            if (!UpdateBoxPosition(pair.Value, pair.Key.bounds))
                toRemove.Add(pair.Key);
        }

        foreach (var col in toRemove)
            HideBox(col);
    }

    private bool UpdateBoxPosition(RectTransform box, Bounds bounds)
    {
        Vector3[] corners = new Vector3[]
        {
            bounds.min,
            bounds.max,
            new(bounds.min.x, bounds.max.y, bounds.min.z),
            new(bounds.max.x, bounds.min.y, bounds.max.z),
            new(bounds.min.x, bounds.min.y, bounds.max.z),
            new(bounds.max.x, bounds.max.y, bounds.min.z),
            new(bounds.min.x, bounds.max.y, bounds.max.z),
            new(bounds.max.x, bounds.min.y, bounds.min.z)
        };

        Vector3 min = new(float.MaxValue, float.MaxValue), max = new(float.MinValue, float.MinValue);
        bool anyVisible = false;

        foreach (var corner in corners)
        {
            Vector3 screen = mainCamera.WorldToScreenPoint(corner);
            if (screen.z > 0)
            {
                anyVisible = true;
                min = Vector3.Min(min, screen);
                max = Vector3.Max(max, screen);
            }
        }

        if (!anyVisible || max.x < 0 || max.y < 0 || min.x > Screen.width || min.y > Screen.height)
            return false;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, min, mainCamera, out Vector2 minLocal) &&
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, max, mainCamera, out Vector2 maxLocal))
        {
            Vector2 center = (minLocal + maxLocal) / 2f;
            Vector2 size = (maxLocal - minLocal) * boxSizeMultiplier + Vector2.one * 20f;

            box.anchoredPosition = center;
            box.sizeDelta = size;
        }

        return true;
    }

    private IEnumerator HideBoxAfterDelay(Collider col, float delay)
    {
        yield return new WaitForSeconds(delay);
        HideBox(col);
    }

    private void HideBox(Collider col)
    {
        if (!activeBoxes.TryGetValue(col, out var box)) return;

        activeBoxes.Remove(col);
        box.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() => Destroy(box.gameObject));
    }

    private void SetBoxColor(Transform box, Color color)
    {
        foreach (var img in box.GetComponentsInChildren<Image>(true))
        {
            img.color = new Color(color.r, color.g, color.b, img.color.a);
        }
    }

    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((1 << obj.layer) & layerMask) != 0;
    }
}
