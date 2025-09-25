using UnityEngine;

/// <summary>
/// Comprueba si se puede ver al objetivo: distancia, �ngulo y raycast para obst�culos.
/// </summary>
public class LineOfSight : MonoBehaviour
{
    public float viewDistance = 8f;
    [Range(1, 180)]
    public float viewAngle = 90f;
    public LayerMask obstacleMask;
    public LayerMask targetMask; // normalmente layer del player

    /// <summary>
    /// Devuelve true si target est� en visi�n (angle + distancia) y no hay obst�culos en el medio.
    /// </summary>
    public bool CanSee(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        float dist = dir.magnitude;
        if (dist > viewDistance) return false;
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > viewAngle * 0.5f) return false;

        // Raycast para verificar obst�culos
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 toTarget = target.position - origin;
        if (Physics.Raycast(origin, toTarget.normalized, out hit, viewDistance))
        {
            // Si el primer collider hit es el jugador -> visible
            if (hit.transform == target) return true;
            // sino hay un obst�culo intermedio
            return false;
        }
        return false;
    }
}
