using UnityEngine;

public class FSM<T>
{
    IState<T> current;

    public FSM() { }
    public FSM(IState<T> current)
    {
        this.current = current;
        current.Enter();
    }

    public void SetInitialState(IState<T> current)
    {
        this.current = current;
        current.Enter();
    }

    public void OnUpdate()
    {
        current.Execute();
    }

    public void SetState(T input)
    {
        if (current.GetState(input, out IState<T> newState))
        {
            current.Exit();
            current = newState;
            current.Enter();
        }
    }

}
