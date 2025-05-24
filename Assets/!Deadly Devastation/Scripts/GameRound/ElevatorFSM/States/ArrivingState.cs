using UnityEngine;

namespace RKS.DD.Game.ElevatorStates
{
    public class ArrivingState : ElevatorState
    {
        public ArrivingState(FSMElevator fsm) : base(fsm) { }

        public override void Enter()
        {
            fsm.elevator.SetStage(RoundEvents.ElevatorStage.Arriving);
            RoundEvents.InvokeGenerationUpdated(new GenerationInfo(RoundEvents.GenerationStage.Started, fsm.elevator.chunkManager.currentFloorIndex));
            fsm.elevator.statusText.text = GetRandom(new[]
            {
            "Приехали.", "Сейчас вылезать будете.", "Готовы?",
            "Этаж доставлен. Как и вы.", "Добро пожаловать... в беду.",
            "Живыми вряд ли вернётесь.", "Не благодарите.",
            "Я свою работу сделал, ваша очередь.", "Сейчас открою...", "Не беспокойтесь, вас убьют."
            });
            fsm.elevator.floorText.text = $"ЭТАЖ {fsm.elevator.chunkManager.currentFloorIndex}";
            fsm.elevator.HighlightUI(fsm.elevator.gameStatusBackground, fsm.elevator.originalMaterialStatus);
            fsm.elevator.HighlightUI(fsm.elevator.floorCounterBackground, fsm.elevator.originalMaterialFloor);

            fsm.elevator.WaitAndContinue(fsm.elevator.arrivingDuration, () =>
            {
                fsm.SetState(new RKS.DD.Game.ElevatorStates.DoorOpenedState(fsm));
            });

            string GetRandom(string[] phrases) => phrases[Random.Range(0, phrases.Length)];
        }
    }
}
