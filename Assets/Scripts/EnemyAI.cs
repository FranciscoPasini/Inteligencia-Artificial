using UnityEngine;
using System.Collections;

/// <summary>
/// FSM básico que cumple los estados pedidos: Patrol, Idle, RunAway, Attack.
/// Tiene dos tipos de comportamiento (BehaviorType): Aggressive persigue, Coward huye.
/// Comentarios en el código justifican decisiones (requisito del TP).
/// </summary>
[RequireComponent(typeof(SteeringAgent))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Patrol, Idle, RunAway, Attack }
    public enum BehaviorType { Aggressive, Coward }

    [Header("Componentes")]
    public BehaviorType behavior = BehaviorType.Aggressive;
    public WaypointPatrol patrol; // asignar objeto con waypoints
    SteeringAgent agent;
    public LineOfSight los;
    public Transform player;
    Rigidbody playerRb;

    [Header("Rangos")]
    public float attackRange = 1.5f;     // distancia para 'atacar' (termina el juego)
    public float detectionTimeToLose = 2f; // tiempo para volver a patrullar luego de perder al jugador

    [Header("Patrol / Idle")]
    public int patrolIterationsBeforeIdle = 3;
    int patrolCounter = 0;
    public float idleDuration = 2f;

    EnemyState currentState = EnemyState.Patrol;
    float stateTimer = 0f;
    bool playerVisible = false;

    void Awake()
    {
        agent = GetComponent<SteeringAgent>();
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
        if (player != null) playerRb = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Actualizamos visibilidad con LineOfSight
        if (los != null && player != null)
            playerVisible = los.CanSee(player);

        // FSM transitions
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (playerVisible)
                {
                    // Al detectar, dependiendo del tipo pasamos a Attack o RunAway
                    currentState = (behavior == BehaviorType.Aggressive) ? EnemyState.Attack : EnemyState.RunAway;
                }
                else
                {
                    // Si llegamos al waypoint, avanzamos
                    if (patrol != null && patrol.IsAtWaypoint(transform.position))
                    {
                        patrol.Advance();
                        patrolCounter++;
                        if (patrolCounter >= patrolIterationsBeforeIdle)
                        {
                            currentState = EnemyState.Idle;
                            stateTimer = 0f;
                            patrolCounter = 0;
                        }
                    }
                }
                break;

            case EnemyState.Idle:
                if (playerVisible)
                {
                    currentState = (behavior == BehaviorType.Aggressive) ? EnemyState.Attack : EnemyState.RunAway;
                }
                else
                {
                    stateTimer += Time.deltaTime;
                    if (stateTimer >= idleDuration)
                    {
                        currentState = EnemyState.Patrol;
                    }
                }
                break;

            case EnemyState.RunAway:
                if (!playerVisible)
                {
                    // esperamos un poco antes de volver a patrullar
                    stateTimer += Time.deltaTime;
                    if (stateTimer >= detectionTimeToLose)
                    {
                        currentState = EnemyState.Patrol;
                        stateTimer = 0f;
                    }
                }
                else
                {
                    stateTimer = 0f;
                }
                break;

            case EnemyState.Attack:
                if (!playerVisible)
                {
                    // perder de vista -> volver a patrulla o huir según comportamiento
                    currentState = (behavior == BehaviorType.Aggressive) ? EnemyState.Patrol : EnemyState.Patrol;
                }
                break;
        }
    }

    void FixedUpdate()
    {
        if (agent == null) return;
        Vector3 steering = Vector3.zero;

        // Siempre aplicar obstacle avoidance como prioridad
        Vector3 avoid = agent.ObstacleAvoidance();
        if (avoid.sqrMagnitude > 0.01f) steering += avoid * 1.5f;

        switch (currentState)
        {
            case EnemyState.Patrol:
                if (patrol != null && patrol.CurrentWaypoint != null)
                {
                    steering += agent.Seek(patrol.CurrentWaypoint.position);
                }
                break;

            case EnemyState.Idle:
                // No movemos -- pequeñas oscilaciones podrían ser ok
                break;

            case EnemyState.RunAway:
                if (player != null && playerRb != null)
                {
                    steering += agent.Evade(player, playerRb); // usa Evade para huir de forma inteligente
                }
                break;

            case EnemyState.Attack:
                if (player != null && playerRb != null)
                {
                    // Perseguir con Pursuit
                    steering += agent.Pursuit(player, playerRb);
                    // Si estamos suficientemente cerca -> atacar (termina el juego)
                    float dist = Vector3.Distance(transform.position, player.position);
                    if (dist <= attackRange)
                    {
                        // Attack: terminamos el juego
                        GameManager.Instance.PlayerKilled();
                    }
                }
                break;
        }

        agent.ApplySteering(steering);
    }
}
