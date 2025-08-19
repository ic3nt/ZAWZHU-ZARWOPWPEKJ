using UnityEngine;

[RequireComponent(typeof(PlayerContext))]
public class RunHealthDrain : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField, Tooltip("Damage per second while running")] private float _dps = 2.5f;

    private PlayerContext _ctx;

    private void Awake()
    {
        _ctx = GetComponent<PlayerContext>();
        if (!_health) _health = GetComponent<Health>();
    }

    private void Update()
    {
        if (_ctx?.Movement && _ctx.Movement.IsOwner && _ctx.Movement.IsRunning && _ctx.Movement.IsMoving)
        {
            _health?.TakeDamage(_dps * Time.deltaTime);
        }
    }
}