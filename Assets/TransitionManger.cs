using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace RKS.DD.Core.Managers
{
    public class TransitionManager : MonoBehaviour
    {
        [Header("Transition Settings")]
        [SerializeField] private GameObject transitionPrefab;
        private string transitionInState = "Transition_In";
        private string transitionOutState = "Transition_Out";
        [SerializeField] private float delayBeforeLoad = 0f;

        private Animator _animator;
        private bool _isTransitioning;

        private void Awake()
        {
            if (transitionPrefab == null)
            {
                Debug.LogError("[TransitionManager] Transition prefab not assigned!");
                return;
            }

            var instance = Instantiate(transitionPrefab, transform);
            _animator = instance.GetComponent<Animator>();

            if (_animator == null)
                Debug.LogError("[TransitionManager] Transition prefab must contain an Animator component.");
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
                _animator.Play(transitionInState);

            var inClipLength = GetAnimationLength(transitionInState);
            await Task.Delay(TimeSpan.FromSeconds(inClipLength + delayBeforeLoad));

            var async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;

            while (async.progress < 0.9f)
                await Task.Yield();

            async.allowSceneActivation = true;

            await Task.Yield();

            if (_animator)
                _animator.Play(transitionOutState);

            var outClipLength = GetAnimationLength(transitionOutState);
            await Task.Delay(TimeSpan.FromSeconds(outClipLength));

            _isTransitioning = false;
        }

        private IEnumerator LoadSceneCoroutine(string sceneName)
        {
            _isTransitioning = true;

            if (_animator)
                _animator.Play(transitionInState);

            yield return new WaitForSeconds(GetAnimationLength(transitionInState) + delayBeforeLoad);

            SceneManager.LoadScene(sceneName);

            yield return null;

            if (_animator)
                _animator.Play(transitionOutState);

            yield return new WaitForSeconds(GetAnimationLength(transitionOutState));

            _isTransitioning = false;
        }

        private float GetAnimationLength(string stateName)
        {
            if (_animator == null || _animator.runtimeAnimatorController == null)
                return 0.5f;

            foreach (var clip in _animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == stateName)
                    return clip.length;
            }

            return 0.5f;
        }
    }
}
