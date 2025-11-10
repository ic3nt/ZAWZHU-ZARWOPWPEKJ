using UnityEngine;
using static RKS.DD.Game.Elevator.ElevatorController;

namespace RKS.DD.Game.ElevatorStates
{
    public class DoorClosedState : ElevatorState
    {
        private static readonly string[] Phrases =
        {
        "Пока, пока)!", "Двери закрываются!", "Идите работать!",
        "Работайте!"
        };

        public DoorClosedState(FSMElevator fsm) : base(fsm) { }

        public override void Enter()
        {
            fsm.elevator.SetStage(RoundEvents.ElevatorStage.DoorClosed);
            fsm.elevator.statusText.text = Phrases[Random.Range(0, Phrases.Length)];
            fsm.elevator.playerContainer.SetActive(true);
            fsm.elevator.goodLuckContainer.SetActive(false);
            fsm.elevator.HighlightUI(fsm.elevator.windowsBackground, fsm.elevator.originalMaterialWindow);
            fsm.elevator.HighlightUI(fsm.elevator.gameStatusBackground, fsm.elevator.originalMaterialWindow);
            fsm.elevator.CloseDoor();
            fsm.elevator.TeleportMisplacedPlayers(TeleportTarget.Outside);

            fsm.elevator.WaitUntilConditionMetAndContinue(() => fsm.elevator.roundManager.taskCompleted, () =>
            {
                fsm.SetState(new RKS.DD.Game.ElevatorStates.WaitingForPlayersState(fsm));
            });
        }
    }
}