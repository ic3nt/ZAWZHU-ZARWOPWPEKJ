    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class RoundManager : MonoBehaviour
    {
        [Header("Round Stat")]
        public bool playersInsideElevator;
        public bool taskCompleted = false;

        [Header("Players")]
        public List<GameObject> allPlayers = new List<GameObject>();

        [Header("Managers")]
        [SerializeField] private RKS.DD.Game.ElevatorController elevatorController;

        void Update()
        {
            allPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));

            if (Input.GetKeyDown(KeyCode.P))
            {
                elevatorController.StartSequence();
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                RoundEvents.InvokeTaskCompleted(true);
            }
        }
        private void HandleTaskCompleted(bool isCompleted)
        {
            taskCompleted = isCompleted;

            if (isCompleted)
                Debug.Log("[RoundManager]: Задание выполнено");
            else
                Debug.Log("[RoundManager]: Задание сброшено");
        }

        private void OnEnable()
        {
            RoundEvents.OnTaskCompleted += HandleTaskCompleted;
        }

        private void OnDisable()
        {
            RoundEvents.OnTaskCompleted -= HandleTaskCompleted;
        }
    }
