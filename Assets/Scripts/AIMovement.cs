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

    private void Start()
    {
        // curHealth = maxHealth;
    }

    private void Update()
    {
        currentState = StateMachine.Engage;
        Engage();
        CreatePath();
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
}