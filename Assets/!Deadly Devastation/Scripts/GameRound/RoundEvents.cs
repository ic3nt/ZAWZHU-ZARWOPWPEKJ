using System;
using UnityEngine;

public static class RoundEvents
{
    public enum GenerationStage { Started, Completed, Failed }

    public static Action<GenerationInfo> OnGenerationUpdated;

    public enum ElevatorStage { Beginning, Moving, Arriving, DoorOpened, DoorClosed, WaitingForPlayers }

    public static event Action<ElevatorStage> OnStageChanged;

    public static event Action<bool> OnTaskCompleted;

    public static void InvokeStageChanged(ElevatorStage stage)
    {
        OnStageChanged?.Invoke(stage);
    }

    public static void InvokeGenerationUpdated(GenerationInfo info)
    {
        OnGenerationUpdated?.Invoke(info);
    }

    public static void InvokeTaskCompleted(bool completed)
    {
        OnTaskCompleted?.Invoke(completed);
    }
}
