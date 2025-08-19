using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(PlayerContext))]
public class PlayerLifeController : NetworkBehaviour
{
    [SerializeField] private Health _health;

    private PlayerContext _ctx;

    private void Awake()
    {
        _ctx = GetComponent<PlayerContext>();
        if (!_health) _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.OnDied += OnDied;
        }
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnDied -= OnDied;
        }
    }

    private void OnDied()
    {
        // Player-specific: unlock cursor, disable movement/camera/audio
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (_ctx.Movement) _ctx.Movement.enabled = false;
        if (_ctx.CameraController) _ctx.CameraController.enabled = false;
        if (_ctx.FpAudio) _ctx.FpAudio.enabled = false;
    }
}