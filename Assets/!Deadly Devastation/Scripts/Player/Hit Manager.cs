using System.Collections;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

[RequireComponent(typeof(PlayerContext))]
public class HitManager : MonoBehaviour
{
    [Header("Kick Settings")]
    [SerializeField] private float kickForce = 500f;
    [SerializeField] private float kickRange = 2f;
    [SerializeField] private float delay = 0.2f;
    [SerializeField] private KeyCode kickKey = KeyCode.Q;

    [Header("Kick Animations")]
    [SerializeField] private int kickAnimationCount = 2;

    [Header("Camera Shakes")]
    [SerializeField] private ShakeData kickShakeData;
    [SerializeField] private ShakeData hitShakeData;
    [SerializeField] private ShakeData runningKickShakeData;

    [Header("Special Effects")]
    [SerializeField] private float timeSlowdown = 0.2f;
    [SerializeField] private float slowdownDuration = 0.15f;
    [SerializeField] private float cameraKnockback = 0.2f;

    private Animator _animator;
    private PlayerContext _ctx;
    private PlayerMovement _playerMovement;
    private bool _isKicking = false;

    private void Awake()
    {
        _ctx = GetComponent<PlayerContext>();
        _animator = _ctx.Animator;
        _playerMovement = _ctx.Movement;
    }

    private void Update()
    {
        if (Input.GetKeyDown(kickKey) && !_isKicking)
        {
            StartCoroutine(Kick());
        }
    }

    private IEnumerator Kick()
    {
        _isKicking = true;

        int randomKickIndex = Random.Range(1, kickAnimationCount);
        _animator.SetInteger("KickIndex", randomKickIndex);
        _animator.SetTrigger("Kick");
        Debug.Log("invoke kick" + randomKickIndex);
        bool hitSomething = false;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, kickRange);
        foreach (var hitCollider in hitColliders)
        {
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            if (rb != null && rb.gameObject != this.gameObject)
            {
                rb.AddForce(transform.forward * kickForce);
                hitSomething = true;

                if (hitShakeData != null)
                {
                    CameraShakerHandler.Shake(hitShakeData);
                    StartCoroutine(CameraKnockbackEffect());
                }
            }
        }

        if (!hitSomething && kickShakeData != null)
            CameraShakerHandler.Shake(kickShakeData);

        if (_playerMovement.IsRunning && runningKickShakeData != null)
        {
            CameraShakerHandler.Shake(runningKickShakeData);
            StartCoroutine(DoSlowMotion());
            _playerMovement.AddImpulse(transform.forward * kickForce * 0.01f);
        }

        yield return new WaitForSeconds(delay);
        _isKicking = false;
    }

    private IEnumerator DoSlowMotion()
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = timeSlowdown;
        yield return new WaitForSecondsRealtime(slowdownDuration);
        Time.timeScale = originalTimeScale;
    }

    private IEnumerator CameraKnockbackEffect()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Vector3 originalPos = mainCam.transform.localPosition;
            Vector3 knockbackPos = originalPos - mainCam.transform.forward * cameraKnockback;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 10f;
                mainCam.transform.localPosition = Vector3.Lerp(originalPos, knockbackPos, Mathf.Sin(t * Mathf.PI));
                yield return null;
            }

            mainCam.transform.localPosition = originalPos;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, kickRange);
    }
}
