using UnityEngine;
using static RKS.DD.Game.Elevator.ElevatorController;

namespace RKS.DD.Game.Elevator.States
{
    public class MovingState : ElevatorState
    {
        private static readonly string[] Phrases =
        {
        "Опять вниз?..", "Вы серьёзно?", "Поехали, чудики.",
        "Ну, держитесь.", "Скоро пожалеете.", "Вниз — ваша специализация.",
        "Вы без меня никуда, да?", "Готовы умирать?", "Дно близко."
        };

        public MovingState(FSMElevator fsm) : base(fsm) { }

        public override void Enter()
        {
            fsm.elevator.SetStage(RoundEvents.ElevatorStage.Moving);
            fsm.elevator.statusText.text = Phrases[Random.Range(0, Phrases.Length)];
            fsm.elevator.playerContainer.SetActive(true);
            fsm.elevator.TeleportMisplacedPlayers(TeleportTarget.Inside);
            fsm.elevator.HighlightUI(fsm.elevator.gameStatusBackground, fsm.elevator.originalMaterialStatus);

            fsm.elevator.WaitAndContinue(fsm.elevator.elevatorMoveDuration, () =>
            {
                fsm.SetState(new RKS.DD.Game.Elevator.States.ArrivingState(fsm));
            });
        }
    }
}