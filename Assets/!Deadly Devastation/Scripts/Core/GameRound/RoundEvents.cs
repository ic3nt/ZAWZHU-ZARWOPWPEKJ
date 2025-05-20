using System;
using UnityEngine;

public static class RoundEvents
{
    public enum GenerationStage { Started, Completed, Failed }

    public static Action<GenerationInfo> OnGenerationUpdated;

    public enum ElevatorStage { ElevatorMoving, Arriving, DoorOpened, WaitingInside, WaitingForPlayers }

    public static event Action<ElevatorStage> OnStageChanged;

    public static void InvokeStageChanged(ElevatorStage stage)
    {
        OnStageChanged?.Invoke(stage);
    }

    public static void InvokeGenerationUpdated(GenerationInfo info)
    {
        OnGenerationUpdated?.Invoke(info);
    }
}
