using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RKS.DD.Core.Managers
{
    public class TransitionManager : RKSBehaviour
    {
        [SerializeField] private GameObject transitionPrefab;
        [SerializeField] private Camera transitionUICamera;
        [SerializeField] private float delayBeforeLoad = 0f;

        private readonly string triggerIn = "TransitionIn";
        private readonly string triggerOut = "TransitionOut";

        private Animator _animator;
        private bool _isTransitioning;

        protected override void OnReady()
        {
            transitionUICamera.enabled = false;

            var instance = Instantiate(transitionPrefab, transform);

            var canvas = instance.GetComponentInChildren<Canvas>(true);
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = transitionUICamera;
            }

            _animator = instance.GetComponent<Animator>();
        }

        public void LoadScene(string sceneName)
        {
            _ = LoadSceneAsync(sceneName);
        }

        public async Task LoadSceneAsync(string sceneName)
        {
            if (_isTransitioning)
                return;

            _isTransitioning = true;
            transitionUICamera.enabled = true;

            if (_animator)
                _animator.SetTrigger(triggerIn);

            if (delayBeforeLoad > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBeforeLoad));

            var async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;

            while (async.progress < 0.9f)
                await Task.Yield();

            async.allowSceneActivation = true;
            await Task.Yield();

            if (_animator)
                _animator.SetTrigger(triggerOut);

            await Task.Delay(1000);

            transitionUICamera.enabled = false;
            _isTransitioning = false;
        }
    }
}
