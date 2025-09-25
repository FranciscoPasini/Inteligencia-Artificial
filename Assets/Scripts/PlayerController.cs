using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerTopDown : MonoBehaviour
{
    public float speed = 5f;
    public TMP_Text stateText;

    Rigidbody rb;
    Vector3 input;

    public enum PlayerState { Idle, Walk }
    public PlayerState currentState = PlayerState.Idle;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Evitamos que se caiga o rote raro
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ |
                         RigidbodyConstraints.FreezeRotationY;
    }

    void Update()
    {
        // Leemos flechas/WASD
        float h = Input.GetAxisRaw("Horizontal"); // ? ? 
        float v = Input.GetAxisRaw("Vertical");   // ? ?
        input = new Vector3(h, 0f, v);

        // Cambiar estado
        if (input.sqrMagnitude > 0.01f)
            currentState = PlayerState.Walk;
        else
            currentState = PlayerState.Idle;

        // Texto UI
        if (stateText != null)
            stateText.text = $"Estado jugador: {currentState}";
    }

    void FixedUpdate()
    {
        // Movimiento plano en XZ
        Vector3 move = input.normalized * speed;
        move.y = rb.velocity.y; // mantenemos gravedad si hubiera
        rb.velocity = move;
    }
}
