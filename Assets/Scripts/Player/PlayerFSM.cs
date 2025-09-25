using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private PlayerState currentState;

    void Start()
    {
        ChangeState(new PlayerIdleState(this));
    }

    void Update()
    {
        currentState.Update();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();

        currentState = newState;
        currentState.Enter();
    }
}
