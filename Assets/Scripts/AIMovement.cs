using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AIMovement : MonoBehaviour
{
    // public int maxHealth = 100;
    // public int curHealth;
    // public int panicMultiplier = 1;

    public Node currentNode;
    public List<Node> path = new List<Node>();

    public enum StateMachine
    {
        // Patrol,
        Engage
        // Evade
    }

    public StateMachine currentState;

    public GameObject player;

    public float speed = 3f;
    // limit how many path nodes the AI will traverse per TurnManager-triggered turn
    public int maxStepsPerTurn = 2;

    private void Start()
    {
        // curHealth = maxHealth;
    }

    private void Update()
    {
        // keep debug info but don't drive movement here — AI actions run via TakeTurn()
        Debug.Log("Distance from Player to AI: " + Vector3.Distance(transform.position, player.transform.position));
    }

    void Engage()
    {
        if (path.Count == 0)
        {
            path = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.FindNearestNode(player.transform.position));
        }
    }

    public void CreatePath()
    {
        if (path.Count > 0)
        {
            int x = 0;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[x].transform.position.x, path[x].transform.position.y, path[x].transform.position.z), speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, path[x].transform.position) < 0.1f)
            {
                currentNode = path[x];
                path.RemoveAt(x);
            }
        }
    }

    // Called by TurnManager to perform the enemy's turn as a coroutine
    public IEnumerator TakeTurn()
    {
        int stepsTaken = 0;

        // ensure we have a valid start node
        if (currentNode == null)
        {
            currentNode = AStarManager.instance.FindNearestNode(transform.position);
            Debug.Log("AIMovement: currentNode was null, set to nearest node: " + (currentNode != null ? currentNode.name : "null"));
        }

        Debug.Log("AIMovement.TakeTurn start. currentNode=" + (currentNode!=null?currentNode.name:"null") + ", player=" + (player!=null?player.name:"null"));

        // Take up to maxStepsPerTurn steps; recompute path each step so AI reacts to player
        while (stepsTaken < maxStepsPerTurn)
        {
            path = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.FindNearestNode(player.transform.position));

            if (path == null || path.Count == 0)
            {
                Debug.Log("AIMovement: no path to player or already adjacent. stepsTaken=" + stepsTaken);
                break;
            }

            // remove any leading nodes that are the same as our current node (path may include start)
            while (path.Count > 0 && path[0] == currentNode)
            {
                path.RemoveAt(0);
            }

            if (path.Count == 0)
            {
                Debug.Log("AIMovement: path cleared after removing currentNode. stepsTaken=" + stepsTaken);
                break;
            }

            Node next = path[0];
            Vector3 target = next.transform.position;

            float distBefore = Vector3.Distance(transform.position, target);
            if (distBefore <= 0.01f)
            {
                // already at the next node; consume it but don't count as a moved step
                currentNode = next;
                path.RemoveAt(0);
                Debug.Log("AIMovement: next node equals current position; consuming without counting. currentNode=" + (currentNode!=null?currentNode.name:"null"));
                // continue to try to reach maxStepsPerTurn (but don't increment stepsTaken)
                continue;
            }

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return null;
            }

            currentNode = next;
            path.RemoveAt(0);
            stepsTaken++;
            Debug.Log("AIMovement: completed step " + stepsTaken + "/" + maxStepsPerTurn + ", currentNode=" + (currentNode!=null?currentNode.name:"null"));

            // small yield to allow other systems to update between steps
            yield return null;
        }

        Debug.Log("AIMovement.TakeTurn end. stepsTaken=" + stepsTaken);
        yield break;
    }
}