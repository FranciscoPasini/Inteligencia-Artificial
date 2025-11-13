using System.Collections.Generic;
using UnityEngine;

public class PFNode : MonoBehaviour
{
    public List<PFNode> neighbors = new List<PFNode>();
    public int x, y;
    public int cost = 1;
    public bool isBlocked = false;

    public void Initialize(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Color Color
    {
        set
        {
            GetComponent<Renderer>().material.color = value;
        }
    }

    private void OnMouseDown()
    {
        PathFindingManager.instance.goal = this;
        Color = Color.red;
    }
}
