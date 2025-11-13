using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance { get; private set; }

    [Header("Configuración")]
    public LayerMask obstacleMask;       // Obstáculos que bloquean la vista
    public List<PFNode> allNodes = new List<PFNode>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Cargar automáticamente todos los nodos en la escena
        allNodes.Clear();
        allNodes.AddRange(FindObjectsOfType<PFNode>());
    }

    /// <summary>
    /// Encuentra el nodo más cercano a una posición en el mundo
    /// </summary>
    public PFNode GetClosestNode(Vector3 position)
    {
        PFNode closest = null;
        float minDist = float.MaxValue;

        foreach (var node in allNodes)
        {
            if (node.isBlocked) continue;
            float dist = Vector3.SqrMagnitude(node.transform.position - position);
            if (dist < minDist)
            {
                closest = node;
                minDist = dist;
            }
        }

        return closest;
    }

    /// <summary>
    /// Calcula el camino entre dos puntos usando ThetaStar
    /// </summary>
    public List<PFNode> GetPath(Vector3 startPos, Vector3 goalPos)
    {
        PFNode start = GetClosestNode(startPos);
        PFNode goal = GetClosestNode(goalPos);

        if (start == null || goal == null)
        {
            Debug.LogWarning("No se encontraron nodos cercanos al origen o destino");
            return null;
        }

        var path = Pathfinding.ThetaStar(
            start,
            (n) => n == goal,
            GetNeighborsInSight,
            GetDistanceCost,
            GetDistanceHeuristic,
            HasLineOfSight
        );

        // Colorear el camino para debug
        if (path != null)
        {
            int length = path.Count;
            for (int i = 0; i < length; i++)
            {
                path[i].Color = Color.Lerp(Color.cyan, Color.green, (float)i / length);
            }
        }

        return path;
    }

    // --- Funciones auxiliares ---

    private List<PFNode> GetNeighborsInSight(PFNode node)
    {
        var visible = new List<PFNode>();
        foreach (var neighbor in node.neighbors)
        {
            if (neighbor == null || neighbor.isBlocked) continue;
            if (!Physics.Linecast(node.transform.position, neighbor.transform.position, obstacleMask))
                visible.Add(neighbor);
        }
        return visible;
    }

    private float GetDistanceCost(PFNode from, PFNode to)
    {
        return Vector3.Distance(from.transform.position, to.transform.position);
    }

    private float GetDistanceHeuristic(PFNode node)
    {
        // Heurística simple: distancia hasta el destino global
        return Vector3.Distance(node.transform.position, allNodes[allNodes.Count - 1].transform.position);
    }

    private bool HasLineOfSight(PFNode a, PFNode b)
    {
        return !Physics.Linecast(a.transform.position, b.transform.position, obstacleMask);
    }
}


