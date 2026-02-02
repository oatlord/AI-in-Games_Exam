using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float hopHeight = 0.4f;
    public float rotationSpeed = 720f;
    public float jumpHeight = 1.2f;
    public float tileSize = 1f;

    [Header("Detection Layers")]
    public LayerMask groundMask;
    public LayerMask barrierMask;
    public LayerMask exitMask;

    [Header("References")]
    public TurnManager turnManager;
    public GameObject victoryUI; // Assign your Victory Screen in Inspector
    public Node currentNode;
    public Node victoryNode;

    private InputSystem_Actions input;
    private Coroutine moveCoroutine;
    private bool isMoving;
    private float exitReachDistance = 1.5f;

    void Awake()
    {
        input = new InputSystem_Actions();
    }

    void Start()
    {
        currentNode = AStarManager.instance.FindNearestNode(transform.position);
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.MoveUp.performed += _ => TryMove(Vector3.forward, 0);
        input.Player.MoveRight.performed += _ => TryMove(Vector3.right, 90);
        input.Player.MoveLeft.performed += _ => TryMove(Vector3.left, -90);
        input.Player.MoveDown.performed += _ => TryMove(Vector3.back, 180);
    }

    void OnDisable()
    {
        input.Player.Disable();
    }

    void TryMove(Vector3 dir, float yRotation)
    {
        if (isMoving) return;
        if (turnManager && !turnManager.InputEnabled) return;

        Vector3 targetPos = transform.position + dir * tileSize;
        bool isJumping = false;

        // 1. Check for a barrier in the immediate path
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, out RaycastHit hit, tileSize, barrierMask))
        {
            // 2. Barrier found! Check if neighbor tile is valid ground
            if (Physics.Raycast(targetPos + Vector3.up, Vector3.down, 2f, groundMask))
            {
                isJumping = true; 
                Debug.Log("Wall detected! Initiating jump.");
            }
            else
            {
                return; // No ground to land on
            }
        }
        else
        {
            // Normal move: check ground for the tile we're walking to
            if (!Physics.Raycast(targetPos + Vector3.up, Vector3.down, 2f, groundMask))
                return;
        }

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveRoutine(targetPos, yRotation, isJumping));
    }

    IEnumerator MoveRoutine(Vector3 targetPos, float yRotation, bool isJumping)
{
    isMoving = true;
    Quaternion targetRot = Quaternion.Euler(0, yRotation, 0);

    // 1. Rotate first (keep this as is)
    while (Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        yield return null;
    }

    Vector3 startPos = transform.position;
    float elapsedTime = 0f;
    float duration = Vector3.Distance(startPos, targetPos) / moveSpeed;

    // 2. Movement Loop with Hopping
    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        float percent = Mathf.Clamp01(elapsedTime / duration);

        // Standard linear move from A to B
        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, percent);

        // Determine how high to hop
        // If jumping a barrier, use jumpHeight. Otherwise, use a small hopHeight.
        float currentArcHeight = isJumping ? jumpHeight : hopHeight;

        // Apply the parabolic arc
        // Formula: height * sin(0 to PI) creates a 0 -> 1 -> 0 curve
        float arc = currentArcHeight * Mathf.Sin(percent * Mathf.PI);
        currentPos.y += arc;

        transform.position = currentPos;
        yield return null;
    }

    // Snap to final position
    transform.position = targetPos;
    currentNode = AStarManager.instance.FindNearestNode(transform.position);
    isMoving = false;

    // --- VICTORY CHECK ---
    if (currentNode == victoryNode) 
    {
        yield return StartCoroutine(AutoMoveToExit());
        yield break; 
    }

    if (turnManager)
        turnManager.EndPlayerTurn();
}

    // ===============================
    // EXIT & VICTORY LOGIC
    // ===============================

    IEnumerator AutoMoveToExit()
    {
        // Find the actual exit object near this node
        Collider[] exits = Physics.OverlapSphere(transform.position, exitReachDistance, exitMask);
        if (exits.Length == 0) yield break;
        
        Vector3 exitPos = exits[0].transform.position;
        // Ensure the player stays on the ground level of the exit
        exitPos.y = transform.position.y; 

        // 1. Face the exit
        Vector3 dir = (exitPos - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            while (Quaternion.Angle(transform.rotation, targetRot) > 1f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // 2. Walk directly into the exit center
        // We use a slight hop here too if you want to keep the "bouncy" feel!
        Vector3 startPos = transform.position;
        float walkDuration = 0.5f; 
        float elapsed = 0f;

        while (elapsed < walkDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / walkDuration;
            
            Vector3 currentPos = Vector3.Lerp(startPos, exitPos, percent);
            // Add a final victory hop
            currentPos.y += hopHeight * Mathf.Sin(percent * Mathf.PI);
            
            transform.position = currentPos;
            yield return null;
        }

        // Final position snap
        transform.position = exitPos;

        // Show UI
        if (victoryUI != null) victoryUI.SetActive(true);
        input.Player.Disable();
        
        Debug.Log("Victory achieved on the correct tile!");
    }
    }