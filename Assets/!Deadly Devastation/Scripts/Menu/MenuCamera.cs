using UnityEngine;
using DG.Tweening;
using RKS.DD.Core;

namespace RKS.DD.Menu
{
    public class MenuCamera : RKSBehaviour
    {
        [SerializeField] private float duration;

        public void LookAt(Transform target)
        {
            transform
                .DOLookAt(target.position, duration)
                .SetEase(Ease.OutElastic, overshoot: 1.2f);
        }
    }
}