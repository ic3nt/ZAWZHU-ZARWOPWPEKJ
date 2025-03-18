using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class ObjectDetectorUI : MonoBehaviour
{
    private Camera mainCamera;

    [Header("General")]
    [SerializeField] private LayerMask interactiveLayer;
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private GameObject boundingBoxPrefab;
    [SerializeField] private RectTransform playerCanvas;
    [SerializeField] private float showDuration = 5f;

    [Header("Sounds")]
    [SerializeField] private AudioClip highlightSound;
    [SerializeField] private AudioClip monsterHighlightSound;

    [Header("Frame colours")]
    [SerializeField] private Color interactiveColor = Color.blue;
    [SerializeField] private Color monsterColor = Color.red;

    private Dictionary<Collider, RectTransform> activeBoxes = new();
    private HashSet<Collider> detectedObjects = new();
    private AudioSource audioSource;

    void Start()
    {
        mainCamera = Camera.main;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        DetectObjects();

        foreach (var entry in activeBoxes)
        {
            if (entry.Key)
                UpdateBoundingBoxPosition(entry.Value, entry.Key.bounds);
        }
    }

    void DetectObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 50f, interactiveLayer | monsterLayer);
        detectedObjects.RemoveWhere(col => !System.Array.Exists(colliders, c => c == col));

        foreach (var col in colliders)
        {
            if (detectedObjects.Contains(col)) continue;

            if (!Physics.Linecast(mainCamera.transform.position, col.bounds.center, obstacleLayer))
            {
                detectedObjects.Add(col);
                CreateBoundingBox(col);
            }
        }
    }

    void CreateBoundingBox(Collider col)
    {
        if (!playerCanvas) return;

        bool isMonster = ((1 << col.gameObject.layer) & monsterLayer.value) != 0;
        GameObject newBox = Instantiate(boundingBoxPrefab, playerCanvas);
        RectTransform box = newBox.GetComponent<RectTransform>();

        activeBoxes[col] = box;
        SetColorRecursive(newBox.transform, isMonster ? monsterColor : interactiveColor);

        box.localScale = Vector3.zero;
        box.DOScale(Vector3.one * 1.2f, 0.3f).SetEase(Ease.OutBack);

        audioSource.PlayOneShot(isMonster ? monsterHighlightSound : highlightSound);

        StartCoroutine(HideAfterDelay(col, showDuration));
    }

    void UpdateBoundingBoxPosition(RectTransform box, Bounds bounds)
    {
        Vector3 min = new(float.MaxValue, float.MaxValue), max = new(float.MinValue, float.MinValue);

        foreach (var corner in new Vector3[]
        {
            bounds.min, bounds.max,
            new(bounds.min.x, bounds.min.y, bounds.max.z), new(bounds.min.x, bounds.max.y, bounds.min.z),
            new(bounds.max.x, bounds.min.y, bounds.min.z), new(bounds.max.x, bounds.max.y, bounds.max.z)
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
            HideBoundingBox(col: GetColliderByBox(box));
            return;
        }

        Vector2 minScreenPos, maxScreenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(playerCanvas, min, mainCamera, out minScreenPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(playerCanvas, max, mainCamera, out maxScreenPos);

        Vector2 padding = new(10, 10);
        box.anchoredPosition = (minScreenPos + maxScreenPos) / 2;
        box.sizeDelta = maxScreenPos - minScreenPos + padding * 2;
    }

    IEnumerator HideAfterDelay(Collider col, float delay)
    {
        yield return new WaitForSeconds(delay);
        HideBoundingBox(col);
    }

    void HideBoundingBox(Collider col)
    {
        if (!col || !activeBoxes.TryGetValue(col, out RectTransform box)) return;

        box.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() => Destroy(box.gameObject));
        activeBoxes.Remove(col);
    }

    void SetColorRecursive(Transform parent, Color newColor)
    {
        if (parent.TryGetComponent(out Image img))
            img.color = new Color(newColor.r, newColor.g, newColor.b, img.color.a);

        foreach (Transform child in parent)
            SetColorRecursive(child, newColor);
    }

    Collider GetColliderByBox(RectTransform box)
    {
        foreach (var kvp in activeBoxes)
            if (kvp.Value == box) return kvp.Key;

        return null;
    }
}
