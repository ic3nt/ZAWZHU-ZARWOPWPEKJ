using UnityEngine;

public class FSMElevator : MonoBehaviour
{
    [SerializeField] private ElevatorState currentState;

    public RKS.DD.Game.ElevatorController elevator;

    public void SetState(ElevatorState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    private void Update()
    {
        currentState?.Update();
    }
}
