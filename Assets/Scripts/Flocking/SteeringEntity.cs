using UnityEngine;

public class SteeringEntity : MonoBehaviour
{
    [SerializeField] protected float maxSpeed = 5f;
    [SerializeField] protected float maxForce = 5f;
    protected Vector3 velocity;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Ajustes recomendados para Steering
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = true;   // si querés que toquen el piso realmente
        rb.isKinematic = false;
    }

    public Vector3 Seek(Vector3 position)
    {
        var dir = position - transform.position;
        return Steer(dir.normalized * maxSpeed);
    }

    public Vector3 Steer(Vector3 desired)
    {
        var steering = desired - velocity;
        return Vector3.ClampMagnitude(steering, maxForce * Time.deltaTime);
    }

    public void AddForce(Vector3 force)
    {
        velocity = Vector3.ClampMagnitude(velocity + force, maxSpeed);
    }

    public void Move()
    {
        if (velocity == Vector3.zero) return;

        transform.forward = velocity;

        // Movimiento físico
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
    }
}
