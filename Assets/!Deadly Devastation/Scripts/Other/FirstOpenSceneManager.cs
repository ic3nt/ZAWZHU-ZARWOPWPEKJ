using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using EasyTransition;
using RKS.DD.Core;
using System;

namespace RKS.DD.Core.Managers 
{
    public class FirstOpenSceneManager : RKSBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform toggleAgreeRectTransform;
        [SerializeField] private RectTransform buttonAgreeRectTransform;
        [SerializeField] private Toggle toggleAgree;
        [SerializeField] private Button buttonAgree;

        [Header("UI Animation Settings")]
        [SerializeField] private float middleTogglePosX = 0f;
        [SerializeField] private float rightTogglePosX = 300f;
        [SerializeField] private float downButtonPosY = -200f;
        [SerializeField] private float topButtonPosY = -50f;
        [SerializeField] private float tweenDuration = 0.5f;

        [Header("Transition")]
        //[SerializeField] private DemoLoadScene transitionManager;

        [Header("Camera Rotation")]
        [SerializeField] private Camera cameraToRotate;
        [SerializeField] private float rotationSpeed = 10.0f;

        [Header("Animation Controller")]
        [SerializeField] private Animator animator;

        private float rotationY = 0f;

        protected override void OnReady()
        {
            InitializeSave();
            InitializeCamera();
            InitializeUI();
        }

        private void InitializeCamera()
        {
            if (cameraToRotate == null)
            {
                Debug.LogWarning("[FirstOpenSceneManager] Camera not assigned, skipping rotation setup.");
                return;
            }

            rotationY = cameraToRotate.transform.rotation.eulerAngles.y;
        }

        private void InitializeUI()
        {
            toggleAgreeRectTransform.DOAnchorPosX(middleTogglePosX, tweenDuration);
            buttonAgreeRectTransform.DOAnchorPosY(downButtonPosY, tweenDuration);

            buttonAgree.interactable = false;
            toggleAgree.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void InitializeSave()
        {
            Save.Load();

            if (Save.CurrentData == null)
            {
                Debug.LogWarning("[FirstOpenSceneManager] Save was still null after load → creating new one manually.");
                Save.Write();
            }
        }


        private void Update()
        {
            RotateCamera();
        }

        private void RotateCamera()
        {
            if (cameraToRotate == null) return;

            rotationY += rotationSpeed * Time.deltaTime;
            if (rotationY >= 360f) rotationY -= 360f;

            cameraToRotate.transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            var data = Save.CurrentData;

            if (isOn)
            {
                buttonAgree.interactable = true;
                toggleAgreeRectTransform.DOAnchorPosX(rightTogglePosX, tweenDuration);
                buttonAgreeRectTransform.DOAnchorPosY(topButtonPosY, tweenDuration);

                data.isPlayerAgreedPlay = true;
            }
            else
            {
                buttonAgree.interactable = false;
                toggleAgreeRectTransform.DOAnchorPosX(middleTogglePosX, tweenDuration);
                buttonAgreeRectTransform.DOAnchorPosY(downButtonPosY, tweenDuration);

                data.isPlayerAgreedPlay = false;
            }

            Save.Write();
        }

        public void SelectLocalizationAnimation()
        {
            if (animator != null)
                animator.SetTrigger("IsSelectLocalizationTrigger");
        }

        public void EndAnimation()
        {
            if (animator != null)
                animator.SetTrigger("IsEndTrigger");
        }

        public void GoToMenuScene()
        {
            var data = Save.CurrentData;
            data.isPlayerAgreedPlay = true;
            Save.Write(data);

            Transition?.LoadScene("IsMenuScene");

            Debug.Log("[FirstOpenSceneManager] Player agreed → loading menu.");
        }
    }
}
