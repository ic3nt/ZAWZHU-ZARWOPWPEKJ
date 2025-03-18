using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class ObjectDetectorUI : MonoBehaviour
{
    private Camera mainCamera;
    public LayerMask interactiveLayer;
    public LayerMask monsterLayer;
    public LayerMask obstacleLayer;
    public GameObject boundingBoxPrefab;
    private RectTransform playerCanvas;
    public float showDuration = 5f;
    public AudioClip highlightSound;

    public Color interactiveColor = Color.blue;
    public Color monsterColor = Color.red;

    private Dictionary<Collider, RectTransform> activeBoxes = new Dictionary<Collider, RectTransform>();
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        DetectObjects();
    }

    void DetectObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 50f, interactiveLayer | monsterLayer);
        List<Collider> detectedColliders = new List<Collider>();

        foreach (var col in colliders)
        {
            Vector3 objectCenter = col.bounds.center;

            if (!Physics.Linecast(mainCamera.transform.position, objectCenter, obstacleLayer))
            {
                UpdateBoundingBox(col);
                detectedColliders.Add(col);
            }
        }

        List<Collider> toRemove = new List<Collider>();
        foreach (var col in activeBoxes.Keys)
        {
            if (!detectedColliders.Contains(col))
            {
                HideBoundingBox(col);
                toRemove.Add(col);
            }
        }

        foreach (var col in toRemove)
        {
            activeBoxes.Remove(col);
        }
    }

    void UpdateBoundingBox(Collider col)
    {
        RectTransform box;
        if (!activeBoxes.ContainsKey(col))
        {
            GameObject newBox = Instantiate(boundingBoxPrefab, playerCanvas);
            box = newBox.GetComponent<RectTransform>();
            activeBoxes[col] = box;

            // Выбираем цвет рамки
            Image boxImage = box.GetComponent<Image>();
            if (col.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                boxImage.color = monsterColor;
            }
            else
            {
                boxImage.color = interactiveColor;
            }

            box.localScale = Vector3.zero;
            box.DOScale(Vector3.one * 1.2f, 0.3f).SetEase(Ease.OutBack);

            if (highlightSound != null)
                audioSource.PlayOneShot(highlightSound);

            StartCoroutine(HideAfterDelay(col, showDuration));
        }
        else
        {
            box = activeBoxes[col];
        }

        Vector3[] objectCorners = new Vector3[8];
        Bounds bounds = col.bounds;

        objectCorners[0] = mainCamera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z));
        objectCorners[1] = mainCamera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
        objectCorners[2] = mainCamera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z));
        objectCorners[3] = mainCamera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z));
        objectCorners[4] = mainCamera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z));
        objectCorners[5] = mainCamera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
        objectCorners[6] = mainCamera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
        objectCorners[7] = mainCamera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.max.z));

        float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;

        foreach (var corner in objectCorners)
        {
            if (corner.z > 0)
            {
                minX = Mathf.Min(minX, corner.x);
                minY = Mathf.Min(minY, corner.y);
                maxX = Mathf.Max(maxX, corner.x);
                maxY = Mathf.Max(maxY, corner.y);
            }
        }

        if (maxX < 0 || maxY < 0 || minX > Screen.width || minY > Screen.height)
        {
            box.gameObject.SetActive(false);
            return;
        }
        else
        {
            box.gameObject.SetActive(true);
        }

        Vector2 minScreenPos, maxScreenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(playerCanvas, new Vector2(minX, minY), mainCamera, out minScreenPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(playerCanvas, new Vector2(maxX, maxY), mainCamera, out maxScreenPos);

        Vector2 padding = new Vector2(10, 10);
        minScreenPos -= padding;
        maxScreenPos += padding;

        box.anchoredPosition = (minScreenPos + maxScreenPos) / 2;
        box.sizeDelta = maxScreenPos - minScreenPos;
    }

    IEnumerator HideAfterDelay(Collider col, float delay)
    {
        yield return new WaitForSeconds(delay);
        HideBoundingBox(col);
    }

    void HideBoundingBox(Collider col)
    {
        if (activeBoxes.ContainsKey(col))
        {
            RectTransform box = activeBoxes[col];
            box.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                Destroy(box.gameObject);
            });
        }
    }
}
