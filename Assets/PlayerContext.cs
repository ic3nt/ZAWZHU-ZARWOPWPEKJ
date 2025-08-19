using UnityEngine;
using Unity.Netcode;


[DisallowMultipleComponent]
public class PlayerContext : NetworkBehaviour
{
    [Header("Core Refs")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _animator;
    [SerializeField] private Camera _camera;
    [SerializeField] private AudioSource _audio;


    [Header("Optional Refs")]
    [Tooltip("First-person movement logic (optional, but many systems read from it).")]
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerCameraController _cameraController;
    [SerializeField] private FirstPersonAudio _fpAudio; // existing in your project


    public Rigidbody Rb => _rb ? _rb : (_rb = GetComponent<Rigidbody>());
    public Animator Animator => _animator;
    public Camera Camera => _camera;
    public AudioSource Audio => _audio;
    public PlayerMovement Movement => _movement ? _movement : (_movement = GetComponent<PlayerMovement>());
    public PlayerCameraController CameraController => _cameraController ? _cameraController : (_cameraController = GetComponent<PlayerCameraController>());
    public FirstPersonAudio FpAudio => _fpAudio;


    private void Reset()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _camera = GetComponentInChildren<Camera>();
        _audio = GetComponentInChildren<AudioSource>();
        _movement = GetComponent<PlayerMovement>();
        _cameraController = GetComponent<PlayerCameraController>();
        _fpAudio = GetComponentInChildren<FirstPersonAudio>();
    }
}