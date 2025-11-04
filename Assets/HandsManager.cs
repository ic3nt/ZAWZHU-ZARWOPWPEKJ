using UnityEngine;

public class HandsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform handLeft;
    [SerializeField] private Transform handRight;
    [SerializeField] private Transform handLeftPosition;
    [SerializeField] private Transform handRightPosition;

    [Header("Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool handsActive = false;

    private void Update()
    {
        if (!handsActive) return;

        if (handLeft != null && handLeftPosition != null)
        {
            handLeft.position = handLeftPosition.position;
        }

        if (handRight != null && handRightPosition != null)
        {
            handRight.position = handRightPosition.position;
        }
    }

    // ===== Публичные методы =====

    public void SetHandsActive(bool state)
    {
        handsActive = state;



        if (handLeft != null) handLeft.gameObject.SetActive(state);
        if (handRight != null) handRight.gameObject.SetActive(state);
    }

    public bool AreHandsActive() => handsActive;
}