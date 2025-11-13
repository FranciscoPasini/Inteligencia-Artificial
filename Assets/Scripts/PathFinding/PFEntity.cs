using System.Collections.Generic;
using UnityEngine;

public class PFEntity : MonoBehaviour
{
    public PFNode currentNode;
    public List<PFNode> path = new();
    public float moveSpeed = 5f;

    void Start()
    {
        currentNode = PathFindingManager.instance.Closest(transform.position);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            path = PathFindingManager.instance.GetPath(currentNode);

        if (path == null || path.Count == 0) return;

        Vector3 dir = path[0].transform.position - transform.position;
        transform.position += dir.normalized * moveSpeed * Time.deltaTime;

        if (dir.magnitude < 0.2f)
        {
            currentNode = path[0];
            path.RemoveAt(0);
        }
    }
}

