using UnityEngine;
using DG.Tweening;
using RKS.DD.Core;

namespace RKS.DD.Menu
{
    public class MenuCameraPoint : RKSBehaviour
    {
        public float fov = 60f;
        public float duration = 1f;
        public Ease ease = Ease.OutCubic;
    }
}
