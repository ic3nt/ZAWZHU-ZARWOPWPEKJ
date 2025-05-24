using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RKS.DD.Game
{
    public class ElevatorController : MonoBehaviour
    {
        [Header("Components & Settings")]
        public TMP_Text statusText;
        public TMP_Text floorText;
        public Transform door;
        public Transform insideTeleportPoint;
        public Transform outsideTeleportPoint;
        public BoxCollider blockDoorColider;
        [Space(10)]
        public float openHeight = 3f;
        public float duration = 1.5f;

        [Header("Stage Durations (sec)")]
        public float elevatorMoveDuration = 10f;
        public float arrivingDuration = 5f;
        public float doorOpenDuration = 10f;
        public float doorCloseDuration = 3f;

        [Header("UI")]
        public GameObject windowsBackground;
        public GameObject gameStatusBackground;
        public GameObject floorCounterBackground;
        public GameObject playerContainer;
        public GameObject goodLuckContainer;

        [Header("Managers")]
        public ChunkManager chunkManager;
        public RoundManager roundManager;
        public FSMElevator fsm;

        [Header("Visual Feedback")]
        [SerializeField] private Material highlightMaterial;
        [SerializeField] private float highlightDuration = 0.5f;

        [HideInInspector] public Material originalMaterialWindow;
        [HideInInspector] public Material originalMaterialStatus;
        [HideInInspector] public Material originalMaterialFloor;

        private Vector3 originalPos;

        private readonly Dictionary<GameObject, Mask> masks = new();

        private void Start()
        {
            fsm.elevator = this;
        }

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

        public void SetStage(RoundEvents.ElevatorStage stage)
        {
            Debug.Log($"SetStage: {stage}");
            RoundEvents.InvokeStageChanged(stage);
        }

        public void StartSequence()
        {
            fsm.SetState(new RKS.DD.Game.ElevatorStates.BegginingState(fsm));
            Debug.Log("Лифт запущен");
        }

        public void WaitAndContinue(float seconds, System.Action nextState)
        {
            StartCoroutine(WaitCoroutine(seconds, nextState));
        }

        public void WaitUntilConditionMetAndContinue(System.Func<bool> condition, System.Action next)
        {
            StartCoroutine(WaitUntilCoroutine(condition, next));
        }

        private IEnumerator WaitUntilCoroutine(System.Func<bool> condition, System.Action next)
        {
            while (!condition())
            {
                Debug.Log("Ожидание выполнения условий...");
                yield return new WaitForSeconds(1f);
            }

            Debug.Log("Условия выполнены!");
            next?.Invoke();
        }

        private IEnumerator WaitCoroutine(float seconds, System.Action next)
        {
            float timer = seconds;
            while (timer > 0)
            {
                Debug.Log($"Осталось времени: {Mathf.Ceil(timer)} сек");
                yield return new WaitForSeconds(1f);
                timer -= 1f;
            }

            next?.Invoke();
        }

        public void OpenDoor()
        {
            door.DOLocalMoveY(originalPos.y + openHeight, duration).SetEase(Ease.OutQuad);
            blockDoorColider.enabled = false;
        }

        public void CloseDoor()
        {
            door.DOLocalMoveY(originalPos.y, duration).SetEase(Ease.InQuad);
            blockDoorColider.enabled = true;
        }

        public void TeleportMisplacedPlayers(TeleportTarget target)
        {
            if (!TryGetComponent(out Collider elevatorCollider))
            {
                Debug.LogWarning("Elevator has no collider!");
                return;
            }

            Bounds bounds = elevatorCollider.bounds;
            bool shouldBeInside = target == TeleportTarget.Inside;

            foreach (var player in roundManager.allPlayers)
            {
                if (player == null) continue;
                var vars = player.GetComponent<PlayerGameVariables>();
                if (vars == null) continue;

                bool isInside = bounds.Contains(player.transform.position);

                if (isInside != shouldBeInside)
                {
                    player.transform.position = shouldBeInside
                        ? insideTeleportPoint.position
                        : outsideTeleportPoint.position;
                }

                vars.playerInElevator = shouldBeInside;
            }

            CheckAllPlayersElevatorState();
        }

        public bool AreAllPlayersInElevator()
        {
            if (!TryGetComponent(out Collider elevatorCollider))
                return false;

            Bounds bounds = elevatorCollider.bounds;

            foreach (var player in roundManager.allPlayers)
            {
                if (player == null) continue;
                if (!bounds.Contains(player.transform.position))
                    return false;
            }

            return true;
        }

        public void CheckAllPlayersElevatorState()
        {
            roundManager.playersInsideElevator = AreAllPlayersInElevator();

            if (TryGetComponent(out Collider elevatorCollider))
            {
                Bounds bounds = elevatorCollider.bounds;
                foreach (var player in roundManager.allPlayers)
                {
                    if (player == null) continue;
                    var vars = player.GetComponent<PlayerGameVariables>();
                    if (vars == null) continue;
                    vars.playerInElevator = bounds.Contains(player.transform.position);
                }
            }
        }

        public enum TeleportTarget
        {
            Inside,
            Outside
        }


        #region UI Hightlight
        public void HighlightUI(GameObject parent, Material originalMat)
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

        #endregion
    }
}