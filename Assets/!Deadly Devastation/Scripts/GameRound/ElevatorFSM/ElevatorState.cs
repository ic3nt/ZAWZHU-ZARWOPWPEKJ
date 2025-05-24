using UnityEngine;

public abstract class ElevatorState
{
    protected FSMElevator fsm;

    public ElevatorState(FSMElevator fsm)
    {
        this.fsm = fsm;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
