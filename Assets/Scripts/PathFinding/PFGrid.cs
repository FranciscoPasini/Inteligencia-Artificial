using UnityEngine;

public class PFGrid : MonoBehaviour
{
    [SerializeField] PFNode prefab;
    [SerializeField] PFNode[] nodes;
    [SerializeField] int width = 10;
    [SerializeField] int height = 10;
    [SerializeField] float distance = 1f;
    [SerializeField] int baseCost = 1;

    public PFNode[] Nodes => nodes;

    [ContextMenu("Instantiate Nodes")]
    public void InstantiateGrid()
    {
        nodes = new PFNode[width * height];
        int count = 0;

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                PFNode n = Instantiate(prefab,
                    transform.position + new Vector3(i * distance, 0, j * distance),
                    Quaternion.identity, transform);
                nodes[count] = n;
                n.Initialize(i, j);
                count++;
            }
        }

        for (int i = 0; i < nodes.Length; i++)
        {
            PFNode n = nodes[i];
            if (n.x > 0) n.neighbors.Add(nodes[n.x - 1 + n.y * width]);
            if (n.x < width - 1) n.neighbors.Add(nodes[n.x + 1 + n.y * width]);
            if (n.y > 0) n.neighbors.Add(nodes[n.x + (n.y - 1) * width]);
            if (n.y < height - 1) n.neighbors.Add(nodes[n.x + (n.y + 1) * width]);
        }
    }

    [ContextMenu("Delete Nodes")]
    public void DeleteNodes()
    {
        if (nodes == null) return;
        foreach (var n in nodes)
            if (n != null) DestroyImmediate(n.gameObject);
        nodes = null;
    }
}

