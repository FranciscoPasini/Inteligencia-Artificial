public interface IState<T>
{
    void Enter();
    void Execute();
    void Exit();

    void AddTransition(T input, IState<T> state);
    bool GetState(T input, out IState<T> state);
}
