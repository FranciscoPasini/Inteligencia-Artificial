using System.Collections.Generic;
using UnityEngine;

public class PathFindingManager : MonoBehaviour
{
    public static PathFindingManager instance { get; private set; }

    [Header("References")]
    public PFGrid grid;
    public PFNode goal;
    public LayerMask obstacleMask;

    void Awake() => instance = this;

    public PFNode Closest(Vector3 pos)
    {
        PFNode closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var node in grid.Nodes)
        {
            if (node.isBlocked) continue;
            float dist = Vector3.SqrMagnitude(pos - node.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = node;
            }
        }
        return closest;
    }

    public List<PFNode> GetPath(PFNode start)
    {
        if (goal == null)
        {
            Debug.LogWarning("No hay objetivo asignado para el Pathfinding.");
            return null;
        }

        var path = Pathfinding.ThetaStar(
            start,
            Objective,
            GetNeighborsInSight,
            GetDistanceCost,
            GetDistanceHeuristic,
            HasLineOfSight
        );

        if (path == null) return null;

        int length = path.Count;
        for (int i = 0; i < length; i++)
            path[i].Color = Color.Lerp(Color.cyan, Color.green, (float)i / length);

        return path;
    }

    private bool Objective(PFNode node) => node == goal;

    private List<PFNode> GetNeighborsInSight(PFNode node)
    {
        var visible = new List<PFNode>();
        foreach (var n in node.neighbors)
        {
            if (!n.isBlocked && !Physics.Linecast(node.transform.position, n.transform.position, obstacleMask))
                visible.Add(n);
        }
        return visible;
    }

    private float GetDistanceCost(PFNode a, PFNode b)
        => Vector3.Distance(a.transform.position, b.transform.position);

    private float GetDistanceHeuristic(PFNode node)
        => Vector3.Distance(node.transform.position, goal.transform.position);

    private bool HasLineOfSight(PFNode a, PFNode b)
        => !Physics.Linecast(a.transform.position, b.transform.position, obstacleMask);
}

