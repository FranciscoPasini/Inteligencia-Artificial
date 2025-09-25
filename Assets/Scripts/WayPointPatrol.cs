using UnityEngine;

/// <summary>
/// Sistema de patrulla para un NPC.
/// Recorre una lista de waypoints y al llegar a uno avanza al siguiente.
/// Cuando llega al final, invierte el recorrido (ping-pong).
/// </summary>
public class WaypointPatrol : MonoBehaviour
{
    [Tooltip("Waypoints que definen la ruta")]
    public Transform[] waypoints;

    [Tooltip("Radio de llegada a un waypoint (cuándo se considera alcanzado)")]
    public float waypointRadius = 0.5f;

    [Tooltip("Índice del waypoint actual")]
    public int currentIndex = 0;

    int direction = 1; // 1 = hacia adelante, -1 = hacia atrás

    /// <summary>
    /// Waypoint actual que el NPC debe seguir.
    /// </summary>
    public Transform CurrentWaypoint
    {
        get
        {
            if (waypoints == null || waypoints.Length == 0) return null;
            return waypoints[currentIndex];
        }
    }

    /// <summary>
    /// Llamar cuando el NPC llegue al waypoint.
    /// Avanza el índice y cambia dirección si hace falta.
    /// </summary>
    public void Advance()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        currentIndex += direction;

        if (currentIndex >= waypoints.Length)
        {
            // si llegó al final, cambiar dirección
            direction = -1;
            currentIndex = waypoints.Length - 2; // retroceder al penúltimo
        }
        else if (currentIndex < 0)
        {
            // si volvió al inicio, invertir otra vez
            direction = 1;
            currentIndex = 1;
        }
    }

    /// <summary>
    /// Devuelve true si el NPC está lo suficientemente cerca del waypoint actual.
    /// </summary>
    public bool IsAtWaypoint(Vector3 position)
    {
        var wp = CurrentWaypoint;
        if (wp == null) return true;
        return Vector3.Distance(position, wp.position) <= waypointRadius;
    }
}
