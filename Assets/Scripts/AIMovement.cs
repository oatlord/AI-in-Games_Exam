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

            if (Physics.Linecast(startLine, endLine, out RaycastHit hit, barrierMask))
            {
                Debug.Log("<color=orange>AI bumped into a barrier!</color>");
                forbiddenFrom = currentNode;
                forbiddenTo = nextNode;
                
                // NEW: Cardinal face only
                FaceTargetCardinal(player.transform.position);
                break;
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

                float stretchScaleY = originalScale.y + (Mathf.Sin(percent * Mathf.PI) * (stretchAmount - 1f));
                float stretchScaleXZ = originalScale.x - (Mathf.Sin(percent * Mathf.PI) * 0.1f);
                transform.localScale = new Vector3(stretchScaleXZ, stretchScaleY, stretchScaleXZ);

                yield return null;
            }

            transform.position = targetPos;
            currentNode = nextNode;

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

        isTakingTurn = false;
    }

    // UPDATED: Forces the AI to face only Forward, Back, Left, or Right
    void FaceTargetCardinal(Vector3 target)
    {
        Vector3 diff = target - transform.position;

        // If the horizontal distance is greater than the vertical distance
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
        {
            // Face Left or Right
            float xDir = diff.x > 0 ? 1 : -1;
            transform.rotation = Quaternion.LookRotation(new Vector3(xDir, 0, 0));
        }
        else
        {
            // Face Forward or Back
            float zDir = diff.z > 0 ? 1 : -1;
            transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, zDir));
        }
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