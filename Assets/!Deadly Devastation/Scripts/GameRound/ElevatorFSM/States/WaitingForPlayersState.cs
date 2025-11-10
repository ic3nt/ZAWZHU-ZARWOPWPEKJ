using UnityEngine;
using static RKS.DD.Game.Elevator.ElevatorController;

namespace RKS.DD.Game.ElevatorStates
{
    public class WaitingForPlayersState : ElevatorState
    {
        private static readonly string[] Phrases =
        {
        "Приехали.", "Сейчас вылезать будете.", "Готовы?",
        "Где все?", "Ну же…", "БЫСТРЕЕ!",
        "Я не железный. Хотя...", "Долго ещё?", "Ждём, как всегда.",
        "Я состарюсь тут.", "Ожидание. Моя любимая часть.",
        "Ну давайте, тяните время.", "Ты не один такой тормоз.",
        "Живые? Вау.", "Ну хоть, задание выполнили.", "И сто лет не прошло...",
        "Я скучал. Шутка.", "Скучали? Я — нет.", "Опять вы…",
        "Пятиминутка позора закончена?", "Больно били?",
        "Вернулись потрепанными? Классика.", "Жаль, что вы вернулись.", "Неужели."
        };

        public WaitingForPlayersState(FSMElevator fsm) : base(fsm) { }

        public override void Enter()
        {
            fsm.elevator.SetStage(RoundEvents.ElevatorStage.WaitingForPlayers);
            fsm.elevator.statusText.text = Phrases[Random.Range(0, Phrases.Length)];
            fsm.elevator.HighlightUI(fsm.elevator.gameStatusBackground, fsm.elevator.originalMaterialWindow);
            fsm.elevator.OpenDoor();

            fsm.elevator.WaitUntilConditionMetAndContinue(() => fsm.elevator.AreAllPlayersInElevator(), () =>
            {
                fsm.elevator.CloseDoor();
                fsm.elevator.TeleportMisplacedPlayers(TeleportTarget.Inside);
                fsm.elevator.chunkManager.currentFloorIndex--;
                RoundEvents.InvokeTaskCompleted(false);
                fsm.SetState(new RKS.DD.Game.ElevatorStates.MovingState(fsm));
            });
        }
    }
}