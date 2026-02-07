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

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip hopSfx;
    public AudioClip ribbitSfx;

    private bool isTakingTurn = false;
    private Vector3 originalScale;

    // 🔒 Stored ONLY when last turn ended blocked
    private Node forbiddenFrom = null;
    private Node forbiddenTo = null;

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

        // =========================
        // PLAN PHASE (TURN START)
        // =========================
        path = AStarManager.instance.GeneratePath(
            currentNode,
            playerScript.currentNode,
            forbiddenFrom,
            forbiddenTo
        );

        // Once used, clear memory
        forbiddenFrom = null;
        forbiddenTo = null;

        int stepsTaken = 0;

        // =========================
        // MOVE PHASE (LOCKED)
        // =========================
        while (stepsTaken < maxStepsPerTurn)
        {
            if (path == null || path.Count <= 1)
                break;

            Node nextNode = path[1];

            // Barrier check (NO replanning here)
            Vector3 startLine = currentNode.transform.position + Vector3.up * 0.5f;
            Vector3 endLine = nextNode.transform.position + Vector3.up * 0.5f;

            if (Physics.Linecast(startLine, endLine, barrierMask))
            {
                // Remember the blocked edge for NEXT turn
                forbiddenFrom = currentNode;
                forbiddenTo = nextNode;

                yield return StartCoroutine(
                    FaceTargetCardinalSmooth(player.transform.position)
                );
                break;
            }

            // =========================
            // ROTATION
            // =========================
            Vector3 moveDir = (nextNode.transform.position - transform.position).normalized;
            if (moveDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                while (Quaternion.Angle(transform.rotation, targetRot) > 5f)
                {
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRot,
                        rotationSpeed * Time.deltaTime
                    );
                    yield return null;
                }
            }

            if (audioSource && hopSfx)
                audioSource.PlayOneShot(hopSfx);

            // =========================
            // HOP MOVEMENT
            // =========================
            Vector3 startPos = transform.position;
            Vector3 targetPos = nextNode.transform.position;
            float duration = Vector3.Distance(startPos, targetPos) / speed;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
                pos.y += hopHeight * Mathf.Sin(t * Mathf.PI);
                transform.position = pos;

                float stretch = Mathf.Sin(t * Mathf.PI);
                transform.localScale = new Vector3(
                    originalScale.x - stretch * 0.1f,
                    originalScale.y + stretch * (stretchAmount - 1f),
                    originalScale.z - stretch * 0.1f
                );

                yield return null;
            }

            // =========================
            // LAND
            // =========================
            transform.position = targetPos;
            currentNode = nextNode;

            float squashTime = 0f;
            const float squashDuration = 0.15f;

            while (squashTime < squashDuration)
            {
                squashTime += Time.deltaTime;
                float t = squashTime / squashDuration;

                transform.localScale = new Vector3(
                    originalScale.x * stretchAmount,
                    Mathf.Lerp(originalScale.y * squashAmount, originalScale.y, t),
                    originalScale.z * stretchAmount
                );

                yield return null;
            }

            transform.localScale = originalScale;

            path.RemoveAt(0);
            stepsTaken++;
            yield return new WaitForSeconds(0.1f);
        }

        if (audioSource && ribbitSfx)
            audioSource.PlayOneShot(ribbitSfx);

        // Needed for Undo system
        if (UndoManager.instance)
            UndoManager.instance.RecordState();

        isTakingTurn = false;
    }

    // Used by LoseManager
    public bool GetIsTakingTurn() => isTakingTurn;

    IEnumerator FaceTargetCardinalSmooth(Vector3 target)
    {
        Vector3 diff = target - transform.position;
        Vector3 dir = Mathf.Abs(diff.x) > Mathf.Abs(diff.z)
            ? new Vector3(Mathf.Sign(diff.x), 0, 0)
            : new Vector3(0, 0, Mathf.Sign(diff.z));

        Quaternion targetRot = Quaternion.LookRotation(dir);

        while (Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.rotation = targetRot;
    }

    private void OnDrawGizmos()
    {
        if (currentNode && path != null && path.Count > 1)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                currentNode.transform.position + Vector3.up * 0.5f,
                path[1].transform.position + Vector3.up * 0.5f
            );
        }
    }
}
