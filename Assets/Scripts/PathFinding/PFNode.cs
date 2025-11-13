using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PFNode : MonoBehaviour
{
    public List<PFNode> neighbors = new List<PFNode>();
    public int cost = 1;
    public bool isBlocked = false;

    public Color normalColor = Color.white;
    public Color blockedColor = Color.red;

    public Color Color
    {
        set { GetComponent<Renderer>().material.color = value; }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isBlocked ? blockedColor : normalColor;
        Gizmos.DrawSphere(transform.position, 0.3f);

        // Dibujar líneas a los vecinos
        Gizmos.color = Color.cyan;
        foreach (var neighbor in neighbors)
        {
            if (neighbor != null)
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }
}

