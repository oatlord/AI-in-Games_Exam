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

    public List<Node> GeneratePath(Node start, Node end, Node forbiddenFrom = null, Node forbiddenTo = null, bool prioritizeAxis = true)
    {
        if (start == null || end == null) return null;

        List<Node> openSet = new List<Node>();

        foreach (Node n in FindObjectsOfType<Node>())
        {
            n.gScore = float.MaxValue;
            n.cameFrom = null;
        }

        float dx = Mathf.Abs(start.transform.position.x - end.transform.position.x);
        float dz = Mathf.Abs(start.transform.position.z - end.transform.position.z);
        bool preferHorizontal = dx >= dz;

        start.gScore = 0;
        start.hScore = Heuristic(start, end, preferHorizontal);
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
            openSet.RemoveAt(lowestF);

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

            List<Node> orderedNeighbors = GetOrderedNeighbors(currentNode, end, preferHorizontal);

            foreach (Node connectedNode in orderedNeighbors)
            {
                if (forbiddenFrom != null && forbiddenTo != null)
                {
                    if (currentNode == forbiddenFrom && connectedNode == forbiddenTo)
                        continue;
                }

                // Barrier Check
                Vector3 startPos = currentNode.transform.position + Vector3.up * 0.5f;
                Vector3 endPos = connectedNode.transform.position + Vector3.up * 0.5f;

                if (Physics.Linecast(startPos, endPos, barrierMask))
                    continue;

                float tentativeG = currentNode.gScore + Vector3.Distance(currentNode.transform.position, connectedNode.transform.position);

                if (tentativeG < connectedNode.gScore)
                {
                    connectedNode.cameFrom = currentNode;
                    connectedNode.gScore = tentativeG;
                    connectedNode.hScore = Heuristic(connectedNode, end, preferHorizontal);

                    if (!openSet.Contains(connectedNode))
                        openSet.Add(connectedNode);
                }
            }
        }
        return null;
    }

    float Heuristic(Node a, Node b, bool preferHorizontal)
    {
        float dx = Mathf.Abs(a.transform.position.x - b.transform.position.x);
        float dz = Mathf.Abs(a.transform.position.z - b.transform.position.z);
        float h = dx + dz;

        if (Mathf.Abs(dx - dz) < 0.01f)
        {
            if (preferHorizontal) h += 0.001f * dz;
            else h += 0.001f * dx;
        }
        return h;
    }

    List<Node> GetOrderedNeighbors(Node current, Node end, bool preferHorizontal)
    {
        List<Node> horizontal = new List<Node>();
        List<Node> vertical = new List<Node>();

        foreach (Node n in current.connections)
        {
            if (Mathf.Abs(n.transform.position.x - current.transform.position.x) >
                Mathf.Abs(n.transform.position.z - current.transform.position.z))
                horizontal.Add(n);
            else
                vertical.Add(n);
        }

        horizontal.Sort((a, b) => Mathf.Abs(a.transform.position.x - end.transform.position.x).CompareTo(Mathf.Abs(b.transform.position.x - end.transform.position.x)));
        vertical.Sort((a, b) => Mathf.Abs(a.transform.position.z - end.transform.position.z).CompareTo(Mathf.Abs(b.transform.position.z - end.transform.position.z)));

        return preferHorizontal ? Combine(horizontal, vertical) : Combine(vertical, horizontal);
    }

    List<Node> Combine(List<Node> first, List<Node> second)
    {
        List<Node> result = new List<Node>(first);
        result.AddRange(second);
        return result;
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