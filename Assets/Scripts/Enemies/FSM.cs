public class FSM
{
    private IState currentState;

    public void SetState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void OnUpdate()
    {
        currentState?.Execute();
    }
}
