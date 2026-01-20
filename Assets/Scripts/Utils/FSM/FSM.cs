using System;
using System.Collections.Generic;

public class FSM <T,TState> where TState : Enum
{
    private T owner;
    private IGameState<T> currentStateObject;
    private TState currentState;
    
    public TState CurrentState => currentState;

    public event Action<T, TState> OnStateChanged;

    public FSM(T owner)
    {
        this.owner = owner;
        currentState = default(TState);
    }

    public void ChangeState(TState newState)
    {
        if (currentStateObject != null)
        {
            currentStateObject.Exit(owner);
        }

        currentState = newState;
        currentStateObject = states[newState];
        OnStateChanged?.Invoke(owner, newState);

        if (currentStateObject != null)
        {
            currentStateObject.Enter(owner);
        }
    }

    public void Update()
    {
        if (currentStateObject != null)
        {
            currentStateObject.Update(owner);
        }
    }  

    
    Dictionary<TState, IGameState<T>> states = new Dictionary<TState, IGameState<T>>();
    public void AddState(TState stateType, IGameState<T> state)
    {
        states[stateType] = state;
    }
}

public interface IGameState<T>
{
    void Enter(T owner);
    void Update(T owner);
    void Exit(T owner);
}