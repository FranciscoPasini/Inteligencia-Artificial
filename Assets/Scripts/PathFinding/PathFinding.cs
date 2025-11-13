using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    public static List<T> BFS<T>(T start, Func<T, bool> satisfies, Func<T, List<T>> getNeighbors) where T : class
    {
        Queue<T> frontier = new();
        frontier.Enqueue(start);
        Dictionary<T, T> cameFrom = new();
        cameFrom.Add(start, null);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (satisfies(current))
            {
                List<T> path = new();
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
                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom.Add(next, current);
                }
            }
        }
        return null;
    }

    public static List<T> DFS<T>(T start, Func<T, bool> satisfies, Func<T, List<T>> getNeighbors) where T : class
    {
        Stack<T> frontier = new();
        frontier.Push(start);
        Dictionary<T, T> cameFrom = new();
        cameFrom.Add(start, null);

        while (frontier.Count > 0)
        {
            var current = frontier.Pop();

            if (satisfies(current))
            {
                List<T> path = new();
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
                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Push(next);
                    cameFrom.Add(next, current);
                }
            }
        }
        return null;
    }

    public static List<T> Dijkstra<T>(T start, Func<T, bool> satisfies,
        Func<T, List<T>> getNeighbors, Func<T, T, float> getCost) where T : class
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
                var newCost = costSoFar[current] + getCost(current, next);
                if (!cameFrom.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    var priority = newCost;
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }
        return null;
    }

    public static List<T> GreedyBestFirst<T>(T start, Func<T, bool> satisfies,
        Func<T, List<T>> getNeighbors, Func<T, float> getHeuristic) where T : class
    {
        PriorityQueue<T> frontier = new();
        frontier.Enqueue(start, 0);
        Dictionary<T, T> cameFrom = new();
        cameFrom.Add(start, null);

        while (!frontier.IsEmpty)
        {
            var current = frontier.Dequeue();

            if (satisfies(current))
            {
                List<T> path = new();
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
                if (!cameFrom.ContainsKey(next))
                {
                    var priority = getHeuristic(next);

                    frontier.Enqueue(next, priority);
                    cameFrom.Add(next, current);
                }
            }
        }
        return null;
    }

    public static List<T> Astar<T>(T start, Func<T, bool> satisfies,
        Func<T, List<T>> getNeighbors, Func<T, T, float> getCost, Func<T, float> getHeuristic) where T : class
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
                var newCost = costSoFar[current] + getCost(current, next);
                if (!cameFrom.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    var priority = newCost + getHeuristic(next);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }
        return null;
    }

    public static List<T> AstarPS<T>(T start, Func<T, bool> satisfies, Func<T, List<T>> getNeighbors,
        Func<T, T, float> getCost, Func<T, float> getHeuristic, Func<T, T, bool> lineOfSight) where T : class
    {
        var path = Astar(start, satisfies, getNeighbors, getCost, getHeuristic);
        int current = 0;
        while (current + 2 < path.Count)
        {
            if (lineOfSight(path[current], path[current + 2]))
                path.RemoveAt(current + 1);
            else
                current++;
        }
        return path;

    }

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

    public static IEnumerable<string> ColeccionGenericaDeStrings()
    {
        yield return "Pepe";
        Debug.Log("Pepe");
        yield return "Mario";
        Debug.Log("Mario");
        yield return "Luigi";
        Debug.Log("Luigi");
        yield return "Juan";
        Debug.Log("Juan");
        yield return "Jose";
        Debug.Log("Jose");
    }
}