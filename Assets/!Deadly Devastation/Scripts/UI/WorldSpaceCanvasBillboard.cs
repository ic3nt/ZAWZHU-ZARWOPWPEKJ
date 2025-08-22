using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasGroup))]
public class WorldSpaceCanvasBillboard : MonoBehaviour
{
    [Header("Distance")]
    [SerializeField] float showDistance = 2f;
    [SerializeField] float hideDistance = 2.5f;
    [SerializeField] float fadeSpeed = 5f;

    [Header("Rotation")]
    [SerializeField] bool rotateX = false;
    [SerializeField] bool rotateY = true;
    [SerializeField] bool rotateZ = false;
    [SerializeField] Vector3 rotationOffset = new Vector3(0, 180f, 0);
    [SerializeField] float rotationSmooth = 10f;

    Camera cam;
    CanvasGroup cg;
    bool visible;
    Coroutine fadeRoutine;
    bool autoFade = true;

    void OnValidate()
    {
        if (hideDistance < showDistance) hideDistance = showDistance;
        if (fadeSpeed < 0f) fadeSpeed = 0f;
        if (rotationSmooth < 0f) rotationSmooth = 0f;
    }

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        cam = Camera.main;
        if (cam == null && Camera.allCamerasCount > 0) cam = Camera.allCameras[0];
    }

    void Update()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null && Camera.allCamerasCount > 0) cam = Camera.allCameras[0];
        }

        if (cam != null)
        {
            Vector3 dir = cam.transform.position - transform.position;
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion desired = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(rotationOffset);
                Vector3 de = desired.eulerAngles;
                Vector3 ce = transform.rotation.eulerAngles;

                Quaternion targetRot = Quaternion.Euler(
                    rotateX ? de.x : ce.x,
                    rotateY ? de.y : ce.y,
                    rotateZ ? de.z : ce.z
                );

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSmooth * Time.deltaTime);
            }
        }

        if (!autoFade) return;

        if (cam == null) return;

        float dist = Vector3.Distance(transform.position, cam.transform.position);
        if (!visible && dist <= showDistance) visible = true;
        else if (visible && dist >= hideDistance) visible = false;

        float target = visible ? 1f : 0f;
        cg.alpha = Mathf.MoveTowards(cg.alpha, target, fadeSpeed * Time.deltaTime);
        cg.blocksRaycasts = cg.alpha > 0.01f;
        cg.interactable = cg.alpha > 0.99f;
    }

    public void HideImmediateAndDisableAutoFade()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        autoFade = false;
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }

    public void FadeInAndEnableAutoFade(float duration = 0.8f)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        autoFade = false;
        fadeRoutine = StartCoroutine(FadeTo(1f, duration, true));
    }

    public void FadeOutAndDisableAutoFade(float duration = 0.8f)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        autoFade = false;
        fadeRoutine = StartCoroutine(FadeTo(0f, duration, false));
    }

    IEnumerator FadeTo(float target, float duration, bool enableAutoAfter)
    {
        float start = cg.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, target, duration <= 0f ? 1f : time / duration);
            yield return null;
        }

        cg.alpha = target;
        cg.blocksRaycasts = cg.alpha > 0.01f;
        cg.interactable = cg.alpha > 0.99f;
        fadeRoutine = null;
        autoFade = enableAutoAfter;
    }
}
