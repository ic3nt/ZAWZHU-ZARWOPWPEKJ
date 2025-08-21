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
        if (Input.GetKeyDown(kickKey) && !_isKicking && !HasWallAhead())
        {
            StartCoroutine(Kick());
        }
    }

    private bool HasWallAhead()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, _ctx.WallCheckDistance, _ctx.WallLayer);
    }

    private IEnumerator Kick()
    {
        _isKicking = true;

        AudioManager.Instance.Play("Woosh");

        int randomKickIndex = Random.Range(1, kickAnimationCount);
        _animator.SetInteger("KickIndex", randomKickIndex);
        _animator.SetTrigger("Kick");
        Debug.Log("invoke kick " + randomKickIndex);
        bool hitSomething = false;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, kickRange);
        foreach (var hitCollider in hitColliders)
        {
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            if (rb != null && rb.gameObject != this.gameObject)
            {
                AudioManager.Instance.Play("Hit");
                rb.AddForce(transform.forward * kickForce);
                hitSomething = true;

                if (hitShakeData != null)
                {
                    CameraShakerHandler.Shake(hitShakeData);
                }
            }
        }

        if (!hitSomething && kickShakeData != null)
            CameraShakerHandler.Shake(kickShakeData);

        if (_playerMovement.IsRunning && runningKickShakeData != null)
        {
            CameraShakerHandler.Shake(runningKickShakeData);
            _playerMovement.AddImpulse(transform.forward * kickForce * 0.01f);
        }

        yield return new WaitForSeconds(delay);
        _isKicking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, kickRange);
    }
}
