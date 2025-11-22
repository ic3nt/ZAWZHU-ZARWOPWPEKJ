using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RKS.DD.Core.Managers
{
    public class TransitionManager : RKSBehaviour
    {
        [SerializeField] private GameObject transitionPrefab;
        [SerializeField] private string triggerIn = "TransitionIn";
        [SerializeField] private string triggerOut = "TransitionOut";
        [SerializeField] private float delayBeforeLoad = 0f;

        private Animator _animator;
        private bool _isTransitioning;

        protected override void OnReady()
        {
            if (transitionPrefab == null) return;

            var instance = Instantiate(transitionPrefab, transform);
            _animator = instance.GetComponent<Animator>();
        }

        public void LoadScene(string sceneName)
        {
            if (_isTransitioning) return;
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }

        public async Task LoadSceneAsync(string sceneName)
        {
            if (_isTransitioning) return;
            _isTransitioning = true;

            if (_animator)
                _animator.SetTrigger(triggerIn);

            await Task.Delay(TimeSpan.FromSeconds(delayBeforeLoad));

            var async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;

            while (async.progress < 0.9f)
                await Task.Yield();

            async.allowSceneActivation = true;
            await Task.Yield();

            if (_animator)
                _animator.SetTrigger(triggerOut);

            _isTransitioning = false;
        }

        private IEnumerator LoadSceneCoroutine(string sceneName)
        {
            _isTransitioning = true;

            if (_animator)
                _animator.SetTrigger(triggerIn);

            yield return new WaitForSeconds(delayBeforeLoad);

            SceneManager.LoadScene(sceneName);
            yield return null;

            if (_animator)
                _animator.SetTrigger(triggerOut);

            _isTransitioning = false;
        }
    }
}
