using System.Collections;
using UnityEngine;

public class ElevatorStageManager : MonoBehaviour
{
    [Header("Stage Durations (sec)")]
    public float liftMoveDuration = 10f;
    public float arrivingDuration = 5f;
    public float doorOpenDuration = 10f;
    public float doorCloseDuration = 3f;

    public ChunkManager chunkManager;
    private Coroutine sequenceCoroutine;

    private void Start()
    {
        StartElevatorSequence();
    }

    public void StartElevatorSequence()
    {
        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
            Debug.Log("Корутин остановлен.");
        }
        sequenceCoroutine = StartCoroutine(StageSequence());
        Debug.Log("Корутин запущен.");
    }

    private IEnumerator StageSequence()
    {
        while (true)
        {
            Debug.Log("Стадия: Едем");
            SetStage(RoundEvents.ElevatorStage.ElevatorMoving);
            yield return WaitWithLog(liftMoveDuration);

            Debug.Log("Стадия: Прибыли");
            SetStage(RoundEvents.ElevatorStage.Arriving);
            yield return WaitWithLog(arrivingDuration);

            // Генерация нового этажа
            RoundEvents.InvokeGenerationUpdated(new GenerationInfo(RoundEvents.GenerationStage.Started, chunkManager.currentFloorIndex));

            Debug.Log("Стадия: Открываем дверь");
            SetStage(RoundEvents.ElevatorStage.DoorOpened);
            yield return WaitWithLog(doorOpenDuration);

            Debug.Log("Стадия: Закрываем дверь");
            SetStage(RoundEvents.ElevatorStage.WaitingInside);
            yield return WaitWithLog(doorCloseDuration);

            chunkManager.currentFloorIndex--;
            Debug.Log($"Переходим на этаж {chunkManager.currentFloorIndex}");
        }
    }

    private IEnumerator WaitWithLog(float seconds)
    {
        float timeLeft = seconds;
        while (timeLeft > 0)
        {
            Debug.Log($"Осталось времени: {Mathf.Ceil(timeLeft)} сек");
            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
        }
    }

    private void SetStage(RoundEvents.ElevatorStage stage)
    {
        Debug.Log($"SetStage: {stage}");
        RoundEvents.InvokeStageChanged(stage);
    }
}
