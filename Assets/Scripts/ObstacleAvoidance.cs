using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    public float predictionRange;
    public float radius;
    public LayerMask obstacleMask;

    private Vector3 velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    public Vector3 Avoid(Vector3 velocity)
    {
        this.velocity = velocity;
        if (!Physics.SphereCast(transform.position, radius, velocity, out RaycastHit hit, predictionRange * velocity.magnitude, obstacleMask))
        {
            return Vector3.zero;
        }
        Debug.Log(hit);



        return (transform.position - hit.collider.transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.darkGreen;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.DrawLine(transform.position, Vector3.Cross(velocity, transform.up) * 10);

    }
}