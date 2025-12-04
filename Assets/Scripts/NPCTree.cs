using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ObstacleAvoidance))]
public class NPCTree : MonoBehaviour
{
    public enum EnemyType { Aggressive, Coward }

    [Header("Stats")]
    public int health = 100;
    public int maxHealth = 100;
    [Range(0, 100)] public int lowHealthThreshold = 30;

    [Header("References")]
    public FOV fieldOfView;
    public GameObject target;
    public Transform[] waypoints;

    [Header("Movement")]
    public float attackRange = 2f;
    public float maxSpeed = 5f;
    public float arriveRange = 2f;

    [Header("Behaviour")]
    public EnemyType enemyType = EnemyType.Aggressive;
    public float idleDuration = 2f;
    public float fleeDuration = 5f;

    [Header("Pathfinding (search)")]
    [SerializeField] private float repathTime = 1.0f;   // cada cuanto recalcula ruta
    [SerializeField] public float searchDuration = 10f; // tiempo que busca al perder vision

    [Header("Search Nodes")]
    public float nodeInspectTime = 3f;             // tiempo que revisa un nodo
    public float neighborInspectTime = 1.5f;       // tiempo en nodos vecinos
    public int maxNeighborChecks = 3;              // cantidad máxima de vecinos a revisar

    private PFNode currentSearchNode;
    private Queue<PFNode> neighborQueue = new Queue<PFNode>();
    private float inspectTimer = 0f;
    private bool inspectingNode = false;


    // steering / movement state
    [HideInInspector] public int currentWP = 0;
    [HideInInspector] public Vector3 velocity;
    private Rigidbody rb;
    private float startY;

    // steering helpers
    private Seek seek;
    private Flee flee;
    private Persuit persuit;
    private Evade evade;
    public Arrive arrive;

    // obstacle avoidance
    private ObstacleAvoidance obstacleAvoidance;

    // FSM y estados
    private FSM fsm;
    public NPCIdleState IdleState { get; private set; }
    public NPCPatrolState PatrolState { get; private set; }
    public NPCAttackState AttackState { get; private set; }

    // Pathfinding internals
    private List<PFNode> currentPath = new List<PFNode>();
    private int pathIndex = 0;
    private float repathTimer = 0f;

    // Search internals
    public bool isSearching = false;
    private float searchTimer = 0f;
    public Vector3 lastSeenPosition;

    // utilities
    private float idleTimer = 0f;

    public void StartNodeSearch()
    {
        isSearching = true;

        currentSearchNode = PathfindingManager.Instance.GetClosestNode(lastSeenPosition);

        // si no hay nodo → abortar búsqueda
        if (currentSearchNode == null)
        {
            isSearching = false;
            return;
        }

        neighborQueue.Clear();

        // cargar vecinos seguros
        foreach (var n in currentSearchNode.neighbors)
            if (n != null)
                neighborQueue.Enqueue(n);

        inspectTimer = nodeInspectTime;
        inspectingNode = true;

        // intentar path
        bool ok = RequestPath(currentSearchNode.transform.position);

        // si el path falla → terminar búsqueda
        if (!ok)
        {
            isSearching = false;
        }
    }



    public int UpdateNodeSearch()
    {
        if (!isSearching) return -1;

        if (IsPlayerInSight())
        {
            isSearching = false;
            return 1;
        }

        // Si no tengo path → termina la búsqueda sin crashear
        if (currentPath == null || currentPath.Count == 0)
        {
            isSearching = false;
            return 0;
        }

        // Mientras tenga nodos del camino → seguir
        if (pathIndex < currentPath.Count)
        {
            FollowPath();
            return -1;
        }

        // ------------- LLEGAMOS AL NODO -------------

        // REVISIÓN DEL NODO
        if (inspectingNode)
        {
            inspectTimer -= Time.deltaTime;
            if (inspectTimer <= 0f)
                inspectingNode = false;

            return -1;
        }

        // PASAR A VECINOS
        if (neighborQueue.Count > 0)
        {
            PFNode nextNode = neighborQueue.Dequeue();

            // defensa null
            if (nextNode == null)
                return -1;

            currentSearchNode = nextNode;

            inspectTimer = neighborInspectTime;
            inspectingNode = true;

            bool ok = RequestPath(nextNode.transform.position);

            if (!ok)
            {
                // si no puedo llegar al vecino, sigo con el siguiente
                return -1;
            }

            return -1;
        }

        // FIN DE LA BÚSQUEDA
        isSearching = false;
        return 0;
    }



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        startY = transform.position.y;

        obstacleAvoidance = GetComponent<ObstacleAvoidance>();

        if (waypoints != null && waypoints.Length > 0)
            currentWP = Mathf.Clamp(currentWP, 0, waypoints.Length - 1);

        if (waypoints != null && waypoints.Length > 0)
            arrive = new Arrive(waypoints[currentWP], transform, maxSpeed, arriveRange);

        if (target != null)
        {
            flee = new Flee(target.transform, transform, maxSpeed);
            persuit = new Persuit(target.transform, transform, maxSpeed);
            evade = new Evade(target.transform, transform, maxSpeed);
        }

        // FSM setup
        fsm = new FSM();
        IdleState = new NPCIdleState(this);
        PatrolState = new NPCPatrolState(this);
        AttackState = new NPCAttackState(this);

        fsm.SetState(IdleState);
    }

    void Update()
    {
        // actualizar repathTimer siempre
        if (repathTimer > 0f) repathTimer -= Time.deltaTime;

        fsm.OnUpdate();
    }

    // ----------------------- FSM helpers -----------------------
    public void ChangeState(IState newState) => fsm.SetState(newState);

    // ----------------------- ACTIONS (usadas por estados / tree) -----------------------
    public void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0 || arrive == null) return;

        arrive.SetTarget = waypoints[currentWP];
        var steer = arrive.GetSteerDir(velocity);
        ApplySteering(steer);

        // si llegó al waypoint, lo maneja el PatrolState (ruleta)
        if (Vector3.Distance(transform.position, waypoints[currentWP].position) < 0.5f)
        {
            // avanzar indice para próximo objetivo (si PatrolState decide seguir)
            // El PatrolState actualizará currentWP cuando corresponda.
        }
    }

    public bool IsAtWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return false;
        return Vector3.Distance(transform.position, waypoints[currentWP].position) < 0.5f;
    }

    public void Persuit()
    {
        // Versión simple (steering directo, usado cuando ve al jugador)
        if (persuit == null || target == null) return;
        velocity = persuit.GetSteerDir(velocity);
        ApplySteering(velocity);
    }

    public void PersuitWithPathfinding()
    {
        // llamado cuando queremos usar pathfinding para buscar/perseguir
        if (target == null) return;

        // si todavía ve al jugador: sigue con persuit normal (más reactivo)
        if (IsPlayerInSight())
        {
            Persuit();
            // actualizar última posición vista
            lastSeenPosition = target.transform.position;
            return;
        }

        // si perdió la visión: iniciar o continuar búsqueda
        if (!isSearching)
            StartSearch(lastSeenPosition, searchDuration);

        // mientras busca, sigue el path
        UpdateSearch();
    }

    public void Flee()
    {
        if (evade == null) return;
        velocity = evade.GetSteerDir(velocity);
        ApplySteering(velocity);
    }

    public void Attack()
    {
        Debug.Log($"{name}: Attack!");
        if (target != null)
        {
            Destroy(target);
        }
    }

    public void Die()
    {
        Debug.Log($"{name}: Die");
        Destroy(gameObject);
    }

    // ----------------------- Movement / Steering with avoidance -----------------------
    public void ApplySteering(Vector3 steering)
    {
        Vector3 avoid = Vector3.zero;
        if (obstacleAvoidance != null)
            avoid = obstacleAvoidance.Avoid(steering);

        Vector3 final = steering + avoid;
        final.y = 0f;

        if (final.magnitude > maxSpeed)
            final = final.normalized * maxSpeed;

        velocity = final;

        Vector3 newPos = rb.position + velocity * Time.deltaTime;
        newPos.y = startY;

        rb.MovePosition(newPos);

        if (final.sqrMagnitude > 0.001f)
            transform.forward = Vector3.Slerp(transform.forward, final.normalized, 10f * Time.deltaTime);
    }

    // ----------------------- PATHFINDING -----------------------
    public bool RequestPath(Vector3 goalPosition)
    {
        if (PathfindingManager.Instance == null)
            return false;

        currentPath = PathfindingManager.Instance.GetPath(transform.position, goalPosition);

        if (currentPath == null || currentPath.Count == 0)
        {
            currentPath = null;
            return false;
        }

        pathIndex = 0;
        repathTimer = repathTime;
        return true;
    }



    private void FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0) return;
        if (pathIndex >= currentPath.Count) return;

        PFNode node = currentPath[pathIndex];
        if (node == null)
        {
            pathIndex++;
            return;
        }

        Vector3 nodePos = node.transform.position;

        // usar Arrive hacia el nodo
        if (arrive != null)
            arrive.SetTarget = node.transform;

        var steer = arrive != null ? arrive.GetSteerDir(velocity) : (nodePos - transform.position).normalized * maxSpeed;
        ApplySteering(steer);

        // Si estamos cerca, pasamos al siguiente nodo
        if (Vector3.Distance(transform.position, nodePos) < 0.5f)
            pathIndex++;
    }

    // ----------------------- SEARCH (last seen) logic -----------------------
    public void StartSearch(Vector3 lastPos, float duration)
    {
        isSearching = true;
        searchTimer = duration;
        lastSeenPosition = lastPos;
        RequestPath(lastSeenPosition);
    }

    /// <summary>
    /// Llamar cada frame mientras isSearching == true.
    /// Retorna:
    /// -1 = aún buscando (no encontró, no expiró)
    ///  0 = expiró (falló)
    ///  1 = encontró (volvió a ver al player)
    /// </summary>
    public int UpdateSearch()
    {
        if (!isSearching) return -1;

        // si volvió a ver al jugador
        if (IsPlayerInSight())
        {
            isSearching = false;
            return 1;
        }

        // recalcular ruta si corresponde
        if (repathTimer <= 0f)
        {
            RequestPath(lastSeenPosition);
            repathTimer = repathTime;
        }

        // seguir path
        FollowPath();

        // decrementar timer
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            isSearching = false;
            return 0; // expiró
        }

        return -1; // sigue buscando
    }

    // ----------------------- UTILITIES -----------------------
    public bool IsPlayerInSight()
    {
        return fieldOfView != null && fieldOfView.CheckDetection();
    }

    public bool IsLowHealth()
    {
        return health < maxHealth * lowHealthThreshold / 100f;
    }
}
