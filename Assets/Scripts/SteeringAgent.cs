using UnityEngine;

/// <summary>
/// SteeringAgent aplica fuerzas de steering (Seek, Pursuit, Flee, Evade) y hace avoidance básico
/// mediante raycasts frontales. Ajustá maxSpeed, maxForce y detectionLength en inspector.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class SteeringAgent : MonoBehaviour
{
    [Header("Movimiento")]
    public float maxSpeed = 3.5f;
    public float maxForce = 10f;
    public Rigidbody rb;

    [Header("Obstacle avoidance")]
    public float detectionLength = 2.0f; // cuánto 've' hacia adelante
    public float sphereCastRadius = 0.4f;
    public LayerMask obstacleMask;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        // Aseguramos límite de velocidad
        Vector3 v = rb.velocity;
        Vector3 vFlat = new Vector3(v.x, 0, v.z);
        if (vFlat.magnitude > maxSpeed)
        {
            Vector3 clamped = vFlat.normalized * maxSpeed;
            rb.velocity = new Vector3(clamped.x, rb.velocity.y, clamped.z);
        }
    }

    // --- Steering behaviors (devuelven una fuerza deseada) ---

    public Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desired = (targetPos - transform.position);
        desired.y = 0;
        desired = desired.normalized * maxSpeed;
        Vector3 steer = desired - new Vector3(rb.velocity.x, 0, rb.velocity.z);
        return Vector3.ClampMagnitude(steer, maxForce);
    }

    public Vector3 Flee(Vector3 threatPos)
    {
        Vector3 desired = (transform.position - threatPos);
        desired.y = 0;
        desired = desired.normalized * maxSpeed;
        Vector3 steer = desired - new Vector3(rb.velocity.x, 0, rb.velocity.z);
        return Vector3.ClampMagnitude(steer, maxForce);
    }

    public Vector3 Pursuit(Transform target, Rigidbody targetRb)
    {
        // predecimos la posición futura del objetivo
        Vector3 toTarget = target.position - transform.position;
        float distance = toTarget.magnitude;
        float speed = Mathf.Max(0.01f, rb.velocity.magnitude);
        float lookAhead = distance / (maxSpeed + targetRb.velocity.magnitude + 0.01f);
        Vector3 futurePos = target.position + targetRb.velocity * lookAhead;
        return Seek(futurePos);
    }

    public Vector3 Evade(Transform target, Rigidbody targetRb)
    {
        Vector3 toTarget = target.position - transform.position;
        float distance = toTarget.magnitude;
        float speed = Mathf.Max(0.01f, rb.velocity.magnitude);
        float lookAhead = distance / (maxSpeed + targetRb.velocity.magnitude + 0.01f);
        Vector3 futurePos = target.position + targetRb.velocity * lookAhead;
        return Flee(futurePos);
    }

    public Vector3 ObstacleAvoidance()
    {
        // Lanza un SphereCast hacia adelante; si choca devuelve fuerza lateral para evitarlo.
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        Vector3 dir = transform.forward;
        RaycastHit hit;
        if (Physics.SphereCast(origin, sphereCastRadius, dir, out hit, detectionLength, obstacleMask.value))
        {
            // calculamos un vector de escape: tangente al punto de impacto
            Vector3 away = Vector3.Reflect(dir, hit.normal);
            away.y = 0;
            away = away.normalized * maxForce;
            // aumentar fuerza cuanto más cerca esté el obstáculo
            float weight = 1f - (hit.distance / detectionLength);
            return away * weight * 2f;
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Aplica una fuerza de steering (se recomienda llamar desde EnemyAI)
    /// </summary>
    public void ApplySteering(Vector3 steeringForce)
    {
        // Convertimos a aceleración e aplicamos.
        Vector3 flat = new Vector3(steeringForce.x, 0, steeringForce.z);
        rb.AddForce(flat, ForceMode.Acceleration);
    }
}
