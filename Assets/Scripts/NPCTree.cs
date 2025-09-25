using System;
using UnityEngine;

[RequireComponent(typeof(ObstacleAvoidance))]
[RequireComponent(typeof(Rigidbody))]
public class NPCTree : MonoBehaviour
{
    [SerializeField] float fleeDuration = 5f; // tiempo que huye al detectar al player
    private float fleeTimer;
    private bool isFleeing;

    public enum EnemyType { Aggressive, Coward }

    [Header("Stats")]
    [SerializeField] int health = 100;
    [SerializeField] int maxHealth = 100;
    [SerializeField, Range(0, 100)] int lowHealthThreshold = 30;

    [Header("References")]
    [SerializeField] FOV fieldOfView;
    [SerializeField] GameObject target;
    [SerializeField] Transform[] waypoints;
    [SerializeField] ObstacleAvoidance obstacleAvoidance;

    [Header("Movement")]
    [SerializeField] int currentWP = 0;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] Vector3 velocity;
    [SerializeField] float arriveRange = 2f;

    [Header("Behaviour")]
    [SerializeField] EnemyType enemyType = EnemyType.Aggressive;
    [SerializeField] float idleDuration = 2f;

    private ITreeNode _rootNode;

    private float idleTimer = 0f;
    private bool patrolForward = true;

    // steering helpers
    private Seek seek;
    private Flee flee;
    private Persuit persuit;
    private Evade evade;
    private Arrive arrive;

    // Rigidbody
    private Rigidbody rb;
    private float startY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // que no se caiga de costado
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        startY = transform.position.y;

        if (obstacleAvoidance == null)
            obstacleAvoidance = GetComponent<ObstacleAvoidance>();

        if (waypoints != null && waypoints.Length > 0)
            currentWP = Mathf.Clamp(currentWP, 0, waypoints.Length - 1);

        if (waypoints != null && waypoints.Length > 0)
            seek = new Seek(waypoints[currentWP].transform, transform, maxSpeed);

        if (target != null)
        {
            flee = new Flee(target.transform, transform, maxSpeed);
            persuit = new Persuit(target.transform, transform, maxSpeed);
            evade = new Evade(target.transform, transform, maxSpeed);
        }

        if (waypoints != null && waypoints.Length > 0)
            arrive = new Arrive(waypoints[currentWP].transform, transform, maxSpeed, arriveRange);

        CreateTree();
    }

    private void CreateTree()
    {
        // Actions
        ActionNode die = new(Die);
        ActionNode patrol = new(Patrol);
        ActionNode idle = new(Idle);
        ActionNode fleeAction = new(Flee);
        ActionNode persuitAction = new(Persuit);
        ActionNode attack = new(Attack);

        // Conditions
        QuestionNode inAttackRange = new(() =>
            target != null && Vector3.Distance(transform.position, target.transform.position) < attackRange, attack, persuitAction);

        QuestionNode arrivedAtPoint = new(() =>
            waypoints != null && waypoints.Length > 0 && Vector3.Distance(transform.position, waypoints[currentWP].position) < 0.3f, idle, patrol);

        if (enemyType == EnemyType.Aggressive)
        {
            QuestionNode lowHealth = new(() => health < maxHealth * lowHealthThreshold / 100, fleeAction, inAttackRange);
            QuestionNode isPInSight = new(fieldOfView != null ? new Func<bool>(fieldOfView.CheckDetection) : (() => false), lowHealth, arrivedAtPoint);
            QuestionNode isAlive = new(() => health <= 0, die, isPInSight);
            _rootNode = isAlive;
        }
        else // Coward
        {
            // Si ya está huyendo, sigue en Flee aunque no te vea
            QuestionNode isStillFleeing = new(() => isFleeing, fleeAction, arrivedAtPoint);

            // Si te ve y no está huyendo, activa el Flee
            QuestionNode isPInSightCoward = new(fieldOfView != null ? new Func<bool>(fieldOfView.CheckDetection) : (() => false), fleeAction, isStillFleeing);

            QuestionNode isAlive = new(() => health <= 0, die, isPInSightCoward);
            _rootNode = isAlive;
        }
    }

    void Update()
    {
        if (_rootNode == null)
            CreateTree();

        _rootNode.Execute();
    }

    #region Actions
    private void Die()
    {
        Debug.Log($"{name}: Die");
        Destroy(gameObject);
    }

    private void Flee()
    {
        if (!isFleeing)
        {
            isFleeing = true;
            fleeTimer = fleeDuration;
            Debug.Log($"{name}: Inicia huida");
        }

        Debug.Log($"{name}: Fleeing...");
        if (evade != null)
        {
            var steer = evade.GetSteerDir(velocity);
            ApplySteering(steer);
        }

        // contar tiempo
        fleeTimer -= Time.deltaTime;
        if (fleeTimer <= 0f)
        {
            isFleeing = false;
            Debug.Log($"{name}: Terminó huida, retomando patrulla...");
            ResumePatrol(); // ?? Reengancha la patrulla
        }
    }

    private void Attack()
    {
        Debug.Log($"{name}: Attack (game over!)");
        if (target != null)
        {
            Destroy(target); // simplificado: elimina al player
        }
    }

    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0 || arrive == null)
            return;

        Debug.Log($"{name}: Patrol (wp {currentWP})");
        arrive.SetTarget = waypoints[currentWP].transform;
        var steer = arrive.GetSteerDir(velocity);
        ApplySteering(steer);
    }

    private void Idle()
    {
        Debug.Log($"{name}: Idle");
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            idleTimer = 0f;

            if (patrolForward) currentWP++;
            else currentWP--;

            if (waypoints != null && waypoints.Length > 0)
            {
                if (currentWP >= waypoints.Length)
                {
                    currentWP = waypoints.Length - 2;
                    patrolForward = false;
                }
                else if (currentWP < 0)
                {
                    currentWP = 1;
                    patrolForward = true;
                }

                arrive.SetTarget = waypoints[currentWP].transform;
            }
        }
    }

    private void Persuit()
    {
        Debug.Log($"{name}: Persuit");
        if (persuit != null)
        {
            var steer = persuit.GetSteerDir(velocity);
            ApplySteering(steer);
        }
    }
    #endregion

    #region Helpers
    private void ResumePatrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // Buscar waypoint más cercano
        float minDist = float.MaxValue;
        int nearest = 0;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            float dist = Vector3.Distance(transform.position, waypoints[i].position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = i;
            }
        }

        currentWP = nearest;
        arrive.SetTarget = waypoints[currentWP];
        Debug.Log($"{name}: Retomando patrulla desde WP {currentWP}");
    }

    private void ApplySteering(Vector3 steering)
    {
        Vector3 avoid = Vector3.zero;
        if (obstacleAvoidance != null)
            avoid = obstacleAvoidance.Avoid(steering);

        Vector3 final = steering + avoid;
        final.y = 0f; // nunca moverse en Y

        velocity = final;

        // movimiento con Rigidbody (respeta colisiones)
        Vector3 newPos = rb.position + velocity * Time.deltaTime;
        newPos.y = startY; // mantener altura fija del suelo
        rb.MovePosition(newPos);

        if (final.sqrMagnitude > 0.001f)
            transform.forward = Vector3.Slerp(transform.forward, final.normalized, 10f * Time.deltaTime);
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (waypoints != null)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypoints.Length; i++)
                if (waypoints[i] != null)
                    Gizmos.DrawSphere(waypoints[i].position, 0.08f);
        }
    }
}

