using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    public static List<T> ThetaStar<T>(T start, Func<T, bool> satisfies, Func<T, List<T>> getNeighbors,
        Func<T, T, float> getCost, Func<T, float> getHeuristic, Func<T, T, bool> lineOfSight) where T : class
    {
        PriorityQueue<T> frontier = new();
        frontier.Enqueue(start, 0);
        Dictionary<T, T> cameFrom = new();
        Dictionary<T, float> costSoFar = new();
        cameFrom.Add(start, null);
        costSoFar.Add(start, 0);

        while (!frontier.IsEmpty)
        {
            var current = frontier.Dequeue();

            if (satisfies(current))
            {
                List<T> path = new List<T>();
                while (current != null)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Reverse();
                return path;
            }
            var neighbors = getNeighbors(current);

            for (int i = 0; i < neighbors.Count; i++)
            {
                var next = neighbors[i];
                var realParent = current;
                if (cameFrom.ContainsKey(current) && cameFrom[current] != null && lineOfSight(next, cameFrom[current]))
                    realParent = cameFrom[current];
                var newCost = costSoFar[realParent] + getCost(realParent, next);
                if (!cameFrom.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    var priority = newCost + getHeuristic(next);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = realParent;
                }
            }
        }
        return null;
    }
}