using System;
using UnityEngine;
using Zenject;
using RKS.DD.Core.Managers;

namespace RKS.DD.Core
{
    public abstract class RKSBehaviour : MonoBehaviour, IDisposable
    {
        protected LocalizationManager Localization { get; private set; }
        protected AudioManager Audio { get; private set; }
        protected DiscordController DiscordRPC { get; private set; }
        protected SaveManager Save { get; private set; }
        protected TransitionManager Transition { get; private set; }

        private bool _isDisposed;

        [Inject]
        public virtual void Construct(
            LocalizationManager localization,
            AudioManager audio,
            DiscordController discord,
            SaveManager save,
            TransitionManager transition)
        {
            Localization = localization;
            Audio = audio;
            DiscordRPC = discord;
            Save = save;
            Transition = transition;

            try { OnInjected(); }
            catch (Exception ex)
            {
                Debug.LogError($"[{GetType().Name}] Error in OnInjected: {ex}");
            }
        }

        protected virtual void Awake() { }

        protected virtual void Start()
        {
            try { OnReady(); }
            catch (Exception ex)
            {
                Debug.LogError($"[{GetType().Name}] Error in OnReady: {ex}");
            }
        }

        protected virtual void Update() { }

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        protected virtual void OnInjected() { }
        protected virtual void OnReady() { }
        protected virtual void OnDisposed() { }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            try { OnDisposed(); }
            catch (Exception ex)
            {
                Debug.LogError($"[{GetType().Name}] Dispose exception: {ex}");
            }
        }

        protected void Log(string message) => Debug.Log($"[{GetType().Name}] {message}");
        protected void Warn(string message) => Debug.LogWarning($"[{GetType().Name}] {message}");
        protected void Error(string message) => Debug.LogError($"[{GetType().Name}] {message}");

        protected void SafeInvoke(Action action)
        {
            try { action?.Invoke(); }
            catch (Exception ex)
            {
                Debug.LogError($"[{GetType().Name}] Exception: {ex}");
            }
        }
    }
}
