using DG.Tweening;
using TMPro;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public TMP_Text statusText;
    public TMP_Text floorText;
    public ChunkManager chunkManager;
    public Transform door;
    public float openHeight = 3f;
    public float duration = 1.5f;

    public GameObject playerContainer;
    public GameObject goodLuckContainer;

    private Vector3 originalPos;

    private void Awake()
    {
        originalPos = door.localPosition;
    }

    private void OnEnable()
    {
        RoundEvents.OnStageChanged += OnStageChanged;
    }

    private void OnDisable()
    {
        RoundEvents.OnStageChanged -= OnStageChanged;
    }

    private void OnStageChanged(RoundEvents.ElevatorStage stage)
    {
        int currentFloor = chunkManager.GetCurrentFloorIndex();

        switch (stage)
        {
            case RoundEvents.ElevatorStage.ElevatorMoving:
                statusText.text = $"≈ƒ”...";
                break;

            case RoundEvents.ElevatorStage.Arriving:
                statusText.text = $"œ–»≈’¿À»!";
                floorText.text = $"›“¿∆ {currentFloor}";
                break;

            case RoundEvents.ElevatorStage.DoorOpened:
                statusText.text = $"–¿¡Œ“¿…“≈!";
                playerContainer.SetActive(false);
                goodLuckContainer.SetActive(true);
                break;

            case RoundEvents.ElevatorStage.WaitingInside:
                statusText.text = $"’Œ–Œÿ¿ﬂ –¿¡Œ“¿!";
                playerContainer.SetActive(true);
                goodLuckContainer.SetActive(false);
                break;

            case RoundEvents.ElevatorStage.WaitingForPlayers:
                statusText.text = $"Œ∆»ƒ¿ﬁ »√–Œ Œ¬";
                break;
        }

        // «‡ÔÛÒÍ‡ÂÏ ‡ÌËÏ‡ˆËË ‰‚ÂÂÈ
        if (stage == RoundEvents.ElevatorStage.DoorOpened)
        {
            Open();
        }
        else if (stage == RoundEvents.ElevatorStage.WaitingInside || stage == RoundEvents.ElevatorStage.WaitingForPlayers)
        {
            Close();
        }
    }

    public void Open()
    {
        door.DOLocalMoveY(originalPos.y + openHeight, duration).SetEase(Ease.OutQuad);
    }

    public void Close()
    {
        door.DOLocalMoveY(originalPos.y, duration).SetEase(Ease.InQuad);
    }
}
