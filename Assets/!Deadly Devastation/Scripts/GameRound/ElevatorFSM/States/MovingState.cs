using UnityEngine;
using static RKS.DD.Game.ElevatorController;

namespace RKS.DD.Game.ElevatorStates
{
    public class MovingState : ElevatorState
    {
        public MovingState(FSMElevator fsm) : base(fsm) { }

        public override void Enter()
        {
            fsm.elevator.SetStage(RoundEvents.ElevatorStage.Moving);
            fsm.elevator.statusText.text = GetRandom(new[]
            {
            "Опять вниз?..", "Вы серьёзно?", "Поехали, чудики.",
            "Ну, держитесь.", "Скоро пожалеете.", "Вниз - ваша специализация.",
            "Вы без меня никуда, да?", "Готовы умирать?", "Дно близко."
            });
            fsm.elevator.playerContainer.SetActive(true);
            fsm.elevator.TeleportMisplacedPlayers(TeleportTarget.Inside);
            fsm.elevator.HighlightUI(fsm.elevator.gameStatusBackground, fsm.elevator.originalMaterialStatus);

            fsm.elevator.WaitAndContinue(fsm.elevator.elevatorMoveDuration, () =>
            {
                fsm.SetState(new RKS.DD.Game.ElevatorStates.ArrivingState(fsm));
            });

            string GetRandom(string[] phrases) => phrases[Random.Range(0, phrases.Length)];
        }
    }
}