using UnityEngine;

namespace RKS.DD.Game.ElevatorStates
{
    public class DoorOpenedState : ElevatorState
    {
        public DoorOpenedState(FSMElevator fsm) : base(fsm) { }

        public override void Enter()
        {
            fsm.elevator.SetStage(RoundEvents.ElevatorStage.DoorOpened);
            fsm.elevator.statusText.text = GetRandom(new[]
            {
            "Ну, марш отсюда.", "Вперёд, мясо!", "Давайте, покажите класс.",
            "Шагайте отсюда...", "Дверь открыта. На выход!", "Надеюсь, вы не вернётесь.",
            "Выход сзади. Удачи... ха.", "Хватит пялиться, двигайтесь.", "Идите и позорьтесь.", "Идите уже, герои."
            });
            fsm.elevator.playerContainer.SetActive(false);
            fsm.elevator.goodLuckContainer.SetActive(true);
            fsm.elevator.HighlightUI(fsm.elevator.windowsBackground, fsm.elevator.originalMaterialWindow);
            fsm.elevator.HighlightUI(fsm.elevator.gameStatusBackground, fsm.elevator.originalMaterialWindow);
            fsm.elevator.OpenDoor();

            fsm.elevator.WaitAndContinue(fsm.elevator.doorOpenDuration, () =>
            {
                fsm.SetState(new RKS.DD.Game.ElevatorStates.DoorClosedState(fsm));
            });

            string GetRandom(string[] phrases) => phrases[Random.Range(0, phrases.Length)];
        }
    }
}