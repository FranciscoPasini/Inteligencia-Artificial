using System.Collections.Generic;
using UnityEngine;

public abstract class State<T> : IState<T>
{
    private Dictionary<T, IState<T>> transitions = new();
    protected FSM<T> _fsm;
    public virtual void Enter()
    {
        Debug.Log("Entered: " + this);
    }

    public virtual void Execute()
    {

    }

    public virtual void Exit()
    {
        Debug.Log("Exited: " + this);
    }

    public void AddTransition(T input, IState<T> state)
    {
        transitions.TryAdd(input, state);
    }
    public bool GetState(T input, out IState<T> state)
    {
        return transitions.TryGetValue(input, out state);
    }
}
