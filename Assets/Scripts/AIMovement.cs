using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();
    public GameObject player;

    [Header("Movement Settings")]
    public float speed = 3f;
    public float hopHeight = 0.4f;
    public float rotationSpeed = 720f;
    public int maxStepsPerTurn = 2;
    public LayerMask barrierMask;

    [Header("Juice Settings")]
    public float stretchAmount = 1.2f;
    public float squashAmount = 0.7f;
    public float juiceSpeed = 15f; 

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip hopSfx;
    public AudioClip ribbitSfx;

    private bool isTakingTurn = false;
    private Node forbiddenFrom;
    private Node forbiddenTo;
    private Vector3 originalScale;
    int replanAttempts = 0;
    int maxReplansPerTurn = 2;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public IEnumerator TakeTurn()
    {
        if (currentNode == null || isTakingTurn || player == null)
            yield break;

        PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
        if (playerScript == null || playerScript.currentNode == null)
            yield break;

        isTakingTurn = true;

        // PLAN PHASE
        path = AStarManager.instance.GeneratePath(
            currentNode,
            playerScript.currentNode,
            forbiddenFrom,
            forbiddenTo
        );

        int stepsTaken = 0;

        // MOVE PHASE
        while (stepsTaken < maxStepsPerTurn)
        {
            if (path == null || path.Count <= 1)
                break;

            Node nextNode = path[1];

            // Barrier Check
            Vector3 startLine = currentNode.transform.position + Vector3.up * 0.5f;
            Vector3 endLine = nextNode.transform.position + Vector3.up * 0.5f;

            if (Physics.Linecast(startLine, endLine, barrierMask))
            {
                forbiddenFrom = currentNode;
                forbiddenTo = nextNode;
                replanAttempts++;

                if (replanAttempts >= maxReplansPerTurn)
                {
                    yield return StartCoroutine(FaceTargetCardinalSmooth(player.transform.position));
                    break;
                }

                path = AStarManager.instance.GeneratePath(
                    currentNode,
                    playerScript.currentNode,
                    forbiddenFrom,
                    forbiddenTo
                );

                if (path == null || path.Count <= 1)
                {
                    yield return StartCoroutine(FaceTargetCardinalSmooth(player.transform.position));
                    break;
                }
                continue;
            }

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

            if (audioSource != null && hopSfx != null)
                audioSource.PlayOneShot(hopSfx);

            Vector3 startPos = transform.position;
            Vector3 targetPos = nextNode.transform.position;
            float elapsedTime = 0f;
            float duration = Vector3.Distance(startPos, targetPos) / speed;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float percent = Mathf.Clamp01(elapsedTime / duration);

                Vector3 currentPos = Vector3.Lerp(startPos, targetPos, percent);
                float arc = hopHeight * Mathf.Sin(percent * Mathf.PI);
                currentPos.y += arc;
                transform.position = currentPos;

                float sY = originalScale.y + (Mathf.Sin(percent * Mathf.PI) * (stretchAmount - 1f));
                float sXZ = originalScale.x - (Mathf.Sin(percent * Mathf.PI) * 0.1f);
                transform.localScale = new Vector3(sXZ, sY, sXZ);

                yield return null;
            }

            transform.position = targetPos;
            currentNode = nextNode;
            forbiddenFrom = null;
            forbiddenTo = null;
            replanAttempts = 0;

            float squashTimer = 0f;
            float squashDuration = 0.15f; 
            while (squashTimer < squashDuration)
            {
                squashTimer += Time.deltaTime;
                float t = squashTimer / squashDuration;
                float currentSquashY = Mathf.Lerp(originalScale.y * squashAmount, originalScale.y, t);
                transform.localScale = new Vector3(originalScale.x * stretchAmount, currentSquashY, originalScale.z * stretchAmount);
                yield return null;
            }
            
            transform.localScale = originalScale;
            path.RemoveAt(0); 
            stepsTaken++;
            yield return new WaitForSeconds(0.1f);
        }

        if (audioSource != null && ribbitSfx != null)
            audioSource.PlayOneShot(ribbitSfx);

        // Keep this: Needed for Undo system
        if (UndoManager.instance != null) UndoManager.instance.RecordState();
        
        isTakingTurn = false;
    }

    // Keep this: Needed for LoseManager to know when to start the Victory Dance
    public bool GetIsTakingTurn() => isTakingTurn;

    IEnumerator FaceTargetCardinalSmooth(Vector3 target)
    {
        Vector3 diff = target - transform.position;
        Vector3 cardinalDir = (Mathf.Abs(diff.x) > Mathf.Abs(diff.z)) 
            ? new Vector3(diff.x > 0 ? 1 : -1, 0, 0) 
            : new Vector3(0, 0, diff.z > 0 ? 1 : -1);

        Quaternion targetRot = Quaternion.LookRotation(cardinalDir);
        while (Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = targetRot;
    }

    private void OnDrawGizmos()
    {
        if (currentNode != null && path != null && path.Count > 1)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(currentNode.transform.position + Vector3.up * 0.5f, path[1].transform.position + Vector3.up * 0.5f);
        }
    }
}