using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager instance;
    
    [Header("Detection Settings")]
    public LayerMask barrierMask;

    private void Awake()
    {
        instance = this;
    }

    // 🔹 OPTION A: forbidden edge parameters
    public List<Node> GeneratePath(
    Node start,
    Node end,
    Node forbiddenFrom = null,
    Node forbiddenTo = null,
    bool prioritizeAxis = false
)
{
    if (start == null || end == null) return null;

    List<Node> openSet = new List<Node>();

    foreach (Node n in FindObjectsOfType<Node>())
    {
        n.gScore = float.MaxValue;
        n.cameFrom = null;
    }

    start.gScore = 0;
    start.hScore = Vector3.Distance(start.transform.position, end.transform.position);
    openSet.Add(start);

    while (openSet.Count > 0)
    {
        int lowestF = 0;
        for (int i = 1; i < openSet.Count; i++)
        {
            if (openSet[i].FScore() < openSet[lowestF].FScore())
                lowestF = i;
        }

        Node currentNode = openSet[lowestF];
        openSet.Remove(currentNode);

        if (currentNode == end)
        {
            List<Node> path = new List<Node>();
            Node temp = end;

            while (temp != null)
            {
                path.Add(temp);
                temp = temp.cameFrom;
            }

            path.Reverse();
            return path;
        }

        foreach (Node connectedNode in currentNode.connections)
        {
            // 🔹 Forbidden edge
            if (forbiddenFrom != null && forbiddenTo != null)
            {
                if (currentNode == forbiddenFrom && connectedNode == forbiddenTo)
                    continue;
            }

            // 🔹 Wall check
            Vector3 startPos = currentNode.transform.position + Vector3.up * 0.5f;
            Vector3 endPos   = connectedNode.transform.position + Vector3.up * 0.5f;

            if (Physics.Linecast(startPos, endPos, out _, barrierMask))
                continue;

            float tentativeG =
                currentNode.gScore +
                Vector3.Distance(currentNode.transform.position, connectedNode.transform.position);

            if (tentativeG < connectedNode.gScore)
            {
                connectedNode.cameFrom = currentNode;
                connectedNode.gScore = tentativeG;

                // 🔹 Base heuristic
                float h = Vector3.Distance(
                    connectedNode.transform.position,
                    end.transform.position
                );

                // 🔥 AXIS PRIORITY BIAS (WITH STICKINESS)
            if (prioritizeAxis)
            {
                bool aiAlignedZ =
                    Mathf.Abs(start.transform.position.z - end.transform.position.z) < 0.1f;

                bool aiAlignedX =
                    Mathf.Abs(start.transform.position.x - end.transform.position.x) < 0.1f;

                bool nodeAlignedZ =
                    Mathf.Abs(connectedNode.transform.position.z - end.transform.position.z) < 0.1f;

                bool nodeAlignedX =
                    Mathf.Abs(connectedNode.transform.position.x - end.transform.position.x) < 0.1f;

                // 🔒 If AI is already aligned, discourage breaking alignment
                if (aiAlignedZ && !nodeAlignedZ)
                {
                    h *= 2.5f; // penalty for leaving Z axis
                }
                else if (aiAlignedX && !nodeAlignedX)
                {
                    h *= 2.5f; // penalty for leaving X axis
                }
                // 🎯 Otherwise, encourage getting aligned
                else if (nodeAlignedZ || nodeAlignedX)
                {
                    h *= 0.3f; // your existing preference
                }
            }

                connectedNode.hScore = h;

                if (!openSet.Contains(connectedNode))
                    openSet.Add(connectedNode);
            }
        }
    }

    return null;
}


    public Node FindNearestNode(Vector3 pos)
    {
        Node foundNode = null;
        float minDistance = float.MaxValue;

        foreach (Node node in FindObjectsOfType<Node>())
        {
            float dist = Vector3.Distance(pos, node.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                foundNode = node;
            }
        }

        return foundNode;
    }
}
