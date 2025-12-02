using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float arriveDistance = 1f;

    private int currentWaypoint = 0;
    private Boid boid;
    private FOV fov;
    private ObstacleAvoidance obstacle;
    private Transform player;
    private Vector3 targetPosition;

    private void Start()
    {
        boid = GetComponent<Boid>();
        fov = GetComponent<FOV>();
        obstacle = GetComponent<ObstacleAvoidance>();

        player = GameObject.FindWithTag("Player").transform;
        targetPosition = waypoints[0].position;
    }

    private void Update()
    {
        // 1. Si ve al jugador → PERSEGUIR
        if (fov.CheckDetection())
        {
            targetPosition = player.position;
        }
        else
        {
            // 2. Patrulla normal
            Patrol();
        }

        // 3. Aplicar Steering hacia target
        Vector3 seekForce = boid.Seek(targetPosition);

        // 4. Evitar obstáculos
        Vector3 avoidForce = obstacle != null ? obstacle.Avoid(boid.Velocity) : Vector3.zero;

        // 5. Sumar fuerzas sin romper flocking
        boid.AddForce(seekForce * 1.2f);       // un poco más fuerte que flocking
        boid.AddForce(avoidForce * 2f);        // evitar obstáculos más prioritario
    }

    private void Patrol()
    {
        float dist = Vector3.Distance(transform.position, waypoints[currentWaypoint].position);

        if (dist < arriveDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }

        targetPosition = waypoints[currentWaypoint].position;
    }
}
