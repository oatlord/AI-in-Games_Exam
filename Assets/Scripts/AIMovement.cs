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

    void Start()
    {
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
        if (currentNode == null)
            currentNode = AStarManager.instance.FindNearestNode(transform.position);

        Node playerNode = AStarManager.instance.FindNearestNode(player.transform.position);

        // Compute path ONCE per turn
        List<Node> path = AStarManager.instance.GeneratePath(currentNode, playerNode);

        if (path == null || path.Count <= 1)
            yield break;

        int steps = Mathf.Min(maxStepsPerTurn, path.Count - 1);

        for (int i = 1; i <= steps; i++)
        {
            Node nextNode = path[i];
            Vector3 targetPos = nextNode.transform.position + Vector3.up;

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    speed * Time.deltaTime
                );
                yield return null;
            }

            transform.position = targetPos;
            currentNode = nextNode;

            yield return null;
        }
    }

}