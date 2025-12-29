using UnityEngine;
using DG.Tweening;
using RKS.DD.Core;
using System.Collections.Generic;

namespace RKS.DD.Menu
{
    public class MenuCamera : RKSBehaviour
    {
        [SerializeField] private Camera cam;

        [Header("Start settings")]
        [SerializeField] private float startFov = 60f;
        [SerializeField] private float startDuration = 0.5f;
        [SerializeField] private Ease startEase = Ease.OutCubic;

        private readonly Stack<MenuCameraPoint> history = new();

        private Tween moveTween;
        private Tween fovTween;

        private MenuCameraPoint currentPoint;
        private MenuCameraPoint startPoint;

        protected override void OnInjected()
        {
            if (cam == null)
                cam = GetComponent<Camera>();

            CreateStartPoint();
        }

        private void CreateStartPoint()
        {
            GameObject go = new GameObject("Camera Start Point");
            go.transform.SetPositionAndRotation(transform.position, transform.rotation);

            startPoint = go.AddComponent<MenuCameraPoint>();
            startPoint.fov = cam.fieldOfView;
            startPoint.duration = startDuration;
            startPoint.ease = startEase;

            currentPoint = startPoint;
        }

        protected override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoBack();
            }
        }

        public void MoveTo(MenuCameraPoint point)
        {
            if (currentPoint != null)
                history.Push(currentPoint);

            currentPoint = point;
            MoveInternal(point);
        }

        private void MoveInternal(MenuCameraPoint point)
        {
            moveTween?.Kill();
            fovTween?.Kill();

            float duration = point.duration;

            Sequence seq = DOTween.Sequence();
            seq.Join(transform.DOMove(point.transform.position, duration));
            seq.Join(transform.DORotateQuaternion(point.transform.rotation, duration));
            seq.SetEase(point.ease);

            moveTween = seq;

            fovTween = cam.DOFieldOfView(point.fov, duration)
                .SetEase(point.ease);
        }

        public void GoBack()
        {
            if (history.Count == 0)
                return;

            MenuCameraPoint prev = history.Pop();
            currentPoint = prev;
            MoveInternal(prev);
        }
    }
}
