using DG.Tweening;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject windowsBackground;
    [SerializeField] private GameObject gameStatusBackground;
    [SerializeField] private GameObject floorCounterBackground;
    [SerializeField] private GameObject playerContainer;
    [SerializeField] private GameObject goodLuckContainer;

    [Header("Visual Feedback")]
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float highlightDuration = 1f;

    private Material originalMaterialWindow;
    private Material originalMaterialStatus;
    private Material originalMaterialFloor;

    private Vector3 originalPos;
    private Coroutine sequenceCoroutine;

    private readonly Dictionary<GameObject, Mask> masks = new();

    private void Awake()
    {
        originalPos = door.localPosition;

        originalMaterialWindow = GetOriginalMaterial(windowsBackground);
        originalMaterialStatus = GetOriginalMaterial(gameStatusBackground);
        originalMaterialFloor = GetOriginalMaterial(floorCounterBackground);

        AddMaskIfAbsent(windowsBackground);
        AddMaskIfAbsent(gameStatusBackground);
        AddMaskIfAbsent(floorCounterBackground);
    }

    private void AddMaskIfAbsent(GameObject parent)
    {
        if (parent == null) return;

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            var go = child.gameObject;
            if (!masks.ContainsKey(go))
            {
                var mask = go.GetComponent<Mask>();
                if (mask == null)
                    mask = go.AddComponent<Mask>();

                masks[go] = mask;
            }
        }
    }



    private Material GetOriginalMaterial(GameObject go)
    {
        if (go == null) return null;
        Image img = go.GetComponent<Image>();
        return img != null ? img.material : null;
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
            Debug.Log("Лифт остановлен");
        }
        sequenceCoroutine = StartCoroutine(StageSequence());
        Debug.Log("Лифт запущен");
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
                statusText.text = GetRandom(new[]
                {
                    "Опять вниз?..", "Вы серьёзно?", "Поехали, чудики.",
                    "Ну, держитесь.", "Скоро пожалеете.", "Вниз - ваша специализация.",
                    "Вы без меня никуда, да?", "Готовы умирать?", "Дно близко."
                });
                playerContainer.SetActive(true);
                HighlightUI(gameStatusBackground, originalMaterialStatus);
                break;

            case RoundEvents.ElevatorStage.Arriving:
                statusText.text = GetRandom(new[]
                {
                    "Приехали.", "Сейчас вылезать будете.", "Готовы?",
                    "Этаж доставлен. Как и вы.", "Добро пожаловать... в беду.",
                    "Живыми вряд ли вернётесь.", "Не благодарите.",
                    "Я свою работу сделал, ваша очередь.", "Сейчас открою...", "Не беспокойтесь, вас убьют."
                });
                floorText.text = $"ЭТАЖ {currentFloor}";
                HighlightUI(gameStatusBackground, originalMaterialStatus);
                HighlightUI(floorCounterBackground, originalMaterialFloor);
                break;

            case RoundEvents.ElevatorStage.DoorOpened:
                statusText.text = GetRandom(new[]
                {
                    "Ну, марш отсюда.", "Вперёд, мясо!", "Давайте, покажите класс.",
                    "Шагайте отсюда...", "Дверь открыта. На выход!", "Надеюсь, вы не вернётесь.",
                    "Выход сзади. Удачи... ха.", "Хватит пялиться, двигайтесь.", "Идите и позорьтесь.", "Идите уже, герои."
                });
                playerContainer.SetActive(false);
                goodLuckContainer.SetActive(true);
                HighlightUI(windowsBackground, originalMaterialWindow);
                HighlightUI(gameStatusBackground, originalMaterialWindow);
                Open();
                break;

            case RoundEvents.ElevatorStage.WaitingInside:
                statusText.text = GetRandom(new[]
                {
                    "Живые? Вау.", "Ну хоть, задание выполнили.", "И сто лет не прошло...",
                    "Я скучал. Шутка.", "Скучали? Я — нет.", "Опять вы…",
                    "Пятиминутка позора закончена?", "Больно били?",
                    "Вернулись потрепанными? Классика.", "Жаль, что вы вернулись.", "Неужели.",
                });
                playerContainer.SetActive(true);
                goodLuckContainer.SetActive(false);
                HighlightUI(windowsBackground, originalMaterialWindow);
                HighlightUI(gameStatusBackground, originalMaterialWindow);
                Close();
                break;

            case RoundEvents.ElevatorStage.WaitingForPlayers:
                statusText.text = GetRandom(new[]
                {
                    "Где все?", "Ну же…", "БЫСТРЕЕ!",
                    "Я не железный. Хотя...", "Долго ещё?", "Ждём, как всегда.",
                    "Я состарюсь тут.", "Ожидание. Моя любимая часть.",
                    "Ну давайте, тяните время.", "Ты не один такой тормоз.",
                });
                HighlightUI(gameStatusBackground, originalMaterialWindow);
                break;
        }

        string GetRandom(string[] phrases) => phrases[Random.Range(0, phrases.Length)];
    }

    public void Open()
    {
        door.DOLocalMoveY(originalPos.y + openHeight, duration).SetEase(Ease.OutQuad);
    }

    public void Close()
    {
        door.DOLocalMoveY(originalPos.y, duration).SetEase(Ease.InQuad);
    }

    #region UI Highlight

    private void HighlightUI(GameObject parent, Material originalMat)
    {
        if (parent == null) return;

        Image img = parent.GetComponent<Image>();
        if (img == null || highlightMaterial == null) return;

        AddMaskIfAbsent(parent);

        SetMaskGraphics(parent, false);

        img.material = highlightMaterial;

        StartCoroutine(RevertMaterialAfterDelay(parent, img, originalMat));
    }

    private IEnumerator RevertMaterialAfterDelay(GameObject parent, Image img, Material originalMat)
    {
        yield return new WaitForSeconds(highlightDuration);

        if (img != null)
            img.material = originalMat;

        SetMaskGraphics(parent, true);
    }

    private void SetMaskGraphics(GameObject parent, bool enabled)
    {
        if (parent == null) return;

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            var go = child.gameObject;
            if (masks.TryGetValue(go, out var mask))
            {
                mask.showMaskGraphic = enabled;
            }
        }
    }


    #endregion
}
