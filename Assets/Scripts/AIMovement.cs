using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();

    public GameObject player;

    public float speed = 3f;
    public float rotationSpeed = 720f;
    public int maxStepsPerTurn = 2;
    public LayerMask barrierMask;

    private bool isTakingTurn = false;

    // 🔹 OPTION A: memory of last blocked edge
    private Node forbiddenFrom;
    private Node forbiddenTo;

public IEnumerator TakeTurn()
{
    if (currentNode == null || isTakingTurn || player == null)
        yield break;

    PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
    if (playerScript == null || playerScript.currentNode == null)
        yield break;

    isTakingTurn = true;

    // 1. PLAN PHASE: Only plan once at the very start of the turn
    // This uses knowledge gained from PREVIOUS turns (forbidden edges)
    path = AStarManager.instance.GeneratePath(
        currentNode,
        playerScript.currentNode,
        forbiddenFrom,
        forbiddenTo
    );

    int stepsTaken = 0;

    // 2. MOVE PHASE: Execute the pre-planned path
    while (stepsTaken < maxStepsPerTurn)
    {
        if (path == null || path.Count <= 1)
            break;

        // We always look at the next node in the ALREADY calculated path
        Node nextNode = path[1];

        // CHECK: Is there a barrier in the way of our pre-planned step?
        Vector3 startLine = currentNode.transform.position + Vector3.up * 0.5f;
        Vector3 endLine = nextNode.transform.position + Vector3.up * 0.5f;

        if (Physics.Linecast(startLine, endLine, out RaycastHit hit, barrierMask))
        {
            Debug.Log("<color=orange>AI bumped into a barrier! Ending turn and remembering for next time.</color>");
            
            // Store the "forbidden edge" so the NEXT turn's GeneratePath knows to avoid it
            forbiddenFrom = currentNode;
            forbiddenTo = nextNode;

            FaceTarget(player.transform.position);
            break; // STOP MOVING IMMEDIATELY. Turn ends here.
        }

        // --- SUCCESSFUL STEP LOGIC ---
        // If we made it here, the path for this specific step is clear.
        
        // (Optional) Clear memory if the AI successfully moves somewhere else
        // forbiddenFrom = null; 
        // forbiddenTo = null;

        // Rotation
        Vector3 moveDir = (nextNode.transform.position - transform.position).normalized;
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            while (Quaternion.Angle(transform.rotation, targetRot) > 5f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // Movement
        Vector3 targetPos = nextNode.transform.position;
        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        currentNode = nextNode;
        
        // Remove the node we just reached so path[1] is always the NEXT step
        path.RemoveAt(0); 
        
        stepsTaken++;
        yield return new WaitForSeconds(0.1f);
    }

    isTakingTurn = false;
}

    void FaceTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    private void OnDrawGizmos()
    {
        if (currentNode != null && path != null && path.Count > 1)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                currentNode.transform.position + Vector3.up * 0.5f,
                path[1].transform.position + Vector3.up * 0.5f
            );
        }
    }
}
