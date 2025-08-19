using UnityEngine;
using UnityEngine.Events;
using TMPro;
using EasyTransition;

public class NetworkUnstable : MonoBehaviour
{
    public enum ConnectivityState { Unknown, Connected, Unreachable }

    [Header("Bindings")]
    [SerializeField] private PlayerContext context; // optional; auto-resolves
    [SerializeField] private PlayerMovement movement; // optional; auto-resolves
    [SerializeField] private PlayerCameraController cameraController; // optional; auto-resolves

    [Header("UI")]
    [SerializeField] private GameObject unstableWindow;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Behavior")]
    [SerializeField, Tooltip("If true, blocks controls and shows UI when network is unreachable.")]
    private bool lockControlsOnUnstable = true;
    [SerializeField, Tooltip("Seconds to wait before returning to menu when network is unreachable.")]
    private float countdownSeconds = 91f;
    [SerializeField, Tooltip("Automatically return to menu when the countdown finishes.")]
    private bool autoReturnToMenu = true;
    [SerializeField] private string menuSceneName = "IsMenuScene";

    [Header("Transition")]
    [SerializeField] private TransitionSettings transition;
    [SerializeField] private float startDelay = 0f;

    [Header("Events")]
    public UnityEvent OnNetworkUnreachable;
    public UnityEvent OnNetworkRestored;

    private float _remaining;
    private bool _timerRunning;
    private ConnectivityState _state = ConnectivityState.Unknown;
    private float _nextPollTime;
    [SerializeField, Tooltip("How often to poll reachability (seconds)")] private float pollInterval = 0.25f;

    private void Awake()
    {
        if (!context) context = GetComponentInParent<PlayerContext>();
        if (!movement) movement = context ? context.Movement : GetComponentInParent<PlayerMovement>();
        if (!cameraController) cameraController = context ? context.CameraController : GetComponentInParent<PlayerCameraController>();
    }

    private void Start()
    {
        _remaining = countdownSeconds;
        SetWindow(false);
        SetControlsEnabled(true);
        EvaluateConnectivity(forceInvoke: true);
    }

    private void Update()
    {
        if (Time.time >= _nextPollTime)
        {
            _nextPollTime = Time.time + pollInterval;
            EvaluateConnectivity();
        }

        if (_timerRunning)
        {
            _remaining -= Time.deltaTime;
            if (_remaining <= 0f)
            {
                _remaining = 0f;
                _timerRunning = false;
                if (autoReturnToMenu)
                    LoadScene(menuSceneName);
            }
            UpdateTimerText();
        }
    }

    private void EvaluateConnectivity(bool forceInvoke = false)
    {
        var reachability = Application.internetReachability;
        var newState = reachability == NetworkReachability.NotReachable ? ConnectivityState.Unreachable : ConnectivityState.Connected;
        if (!forceInvoke && newState == _state) return;
        _state = newState;

        if (newState == ConnectivityState.Unreachable)
            OnBecameUnreachable();
        else
            OnBecameConnected();
    }

    private void OnBecameUnreachable()
    {
        OnNetworkUnreachable?.Invoke();
        SetWindow(true);
        if (lockControlsOnUnstable) SetControlsEnabled(false);
        StartCountdown();
    }

    private void OnBecameConnected()
    {
        OnNetworkRestored?.Invoke();
        SetWindow(false);
        if (lockControlsOnUnstable) SetControlsEnabled(true);
        StopCountdown(reset: true);
    }

    private void StartCountdown()
    {
        _remaining = countdownSeconds;
        _timerRunning = true;
        UpdateTimerText();
    }

    private void StopCountdown(bool reset)
    {
        _timerRunning = false;
        if (reset) _remaining = countdownSeconds;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (!timeText) return;
        var t = Mathf.Max(0f, _remaining);
        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);
        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    private void SetWindow(bool on)
    {
        if (unstableWindow && unstableWindow.activeSelf != on)
            unstableWindow.SetActive(on);
    }

    private void SetControlsEnabled(bool enabled)
    {
        if (movement) movement.enabled = enabled;
        if (cameraController) cameraController.enabled = enabled;
        // If you have audio or other systems in PlayerContext you want to affect, add it here.
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        if (TransitionManager.Instance())
            TransitionManager.Instance().Transition(sceneName, transition, startDelay);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
