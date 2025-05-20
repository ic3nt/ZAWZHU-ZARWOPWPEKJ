using DG.Tweening;
using TMPro;
using System.Collections;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [Header("Components & Settings")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text floorText;
    [SerializeField] private ChunkManager chunkManager;
    [SerializeField] private Transform door;
    [Space(10)]
    public float openHeight = 3f;
    public float duration = 1.5f;

    [Header("Stage Durations (sec)")]
    [SerializeField] private float elevatorMoveDuration = 10f;
    [SerializeField] private float arrivingDuration = 5f;
    [SerializeField] private float doorOpenDuration = 10f;
    [SerializeField] private float doorCloseDuration = 3f;

    [Header("UI")]
    [SerializeField] private GameObject playerContainer;
    [SerializeField] private GameObject goodLuckContainer;

    private Vector3 originalPos;
    private Coroutine sequenceCoroutine;

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
            yield return WaitWithLog(elevatorMoveDuration);

            Debug.Log("Стадия: Прибыли");
            SetStage(RoundEvents.ElevatorStage.Arriving);
            yield return WaitWithLog(arrivingDuration);

            // генерация нового этажа
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

    private void OnStageChanged(RoundEvents.ElevatorStage stage)
    {
        int currentFloor = chunkManager.GetCurrentFloorIndex();

        switch (stage)
        {
            case RoundEvents.ElevatorStage.ElevatorMoving:
                statusText.text = GetRandom(new[] {
            "Опять вниз?..",
            "Ты серьёзно?",
            "Поехали, чудик.",
            "Ну, держись.",
            "Скоро пожалеешь.",
            "Вниз — твоя специализация.",
            "Ты без меня никуда, да?",
            "Я тебе не экскурсовод.",
            "Готов умирать?",
            "Дно близко."
        });
                break;

            case RoundEvents.ElevatorStage.Arriving:
                statusText.text = GetRandom(new[] {
            "Приехали.",
            "Сейчас вылезать будете.",
            "Ну что, трусы.",
            "Этаж доставлен. Как и вы.",
            "Добро пожаловать… в беду.",
            "Живыми вряд ли вернётесь.",
            "Не благодарите.",
            "Я свою работу сделал, ваша очередь.",
            "Сейчас открою...",
            "Не беспокойтесь, вас убьют."
        });
                floorText.text = $"ЭТАЖ {currentFloor}";
                break;

            case RoundEvents.ElevatorStage.DoorOpened:
                statusText.text = GetRandom(new[] {
            "Ну, марш отсюда.",
            "Вперёд, мясо!",
            "Давайте, покажите класс.",
            "Шагайте отсюда...",
            "Дверь открыта. На выход!",
            "Надеюсь, вы не вернётесь.",
            "Выход сзади. Удачи… ха.",
            "Хватит пялиться, двигайся.",
            "Иди и позорься.",
            "Идите уже, герои."
        });
                playerContainer.SetActive(false);
                goodLuckContainer.SetActive(true);
                break;

            case RoundEvents.ElevatorStage.WaitingInside:
                statusText.text = GetRandom(new[] {
            "Живые? Вау.",
            "Ну хоть, задание выполнили.",
            "Боже...",
            "Я скучал. Шутка.",
            "Скучали? Я — нет.",
            "Опять вы…",
            "Пятиминутка позора закончена?",
            "Больно били?",
            "Вернулись потрепанными? Классика.",
            "Жаль, что вы вернулись.",
            "Неужели.",
        });
                playerContainer.SetActive(true);
                goodLuckContainer.SetActive(false);
                break;

            case RoundEvents.ElevatorStage.WaitingForPlayers:
                statusText.text = GetRandom(new[] {
            "Где все?",
            "Ну же…",
            "БЫСТРЕЕ!",
            "Я не железный. Хотя…",
            "Долго ещё?",
            "Ждём, как всегда.",
            "Я состарюсь тут.",
            "Ожидание. Моя любимая часть.",
            "Ну давайте, тяните время.",
            "Ты не один такой тормоз.",
        });
                break;
        }

        string GetRandom(string[] phrases) => phrases[Random.Range(0, phrases.Length)];


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
