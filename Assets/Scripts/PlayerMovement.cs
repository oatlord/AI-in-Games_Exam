using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float hopHeight = 0.4f;
    public float rotationSpeed = 720f;
    public float jumpHeight = 1.2f;
    public float tileSize = 1f;

    [Header("Juice Settings")]
    public float stretchAmount = 1.2f;
    public float squashAmount = 0.7f;
    public float squashDuration = 0.15f;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip hopSfx;
    public AudioClip ribbitSfx;
    public AudioClip jumpSfx;

    [Header("Detection Layers")]
    public LayerMask groundMask;
    public LayerMask barrierMask;
    public LayerMask exitMask;

    [Header("References")]
    public TurnManager turnManager;
    // public GameObject victoryUI;
    public WinManager winManager;
    public Node currentNode;
    public Node victoryNode;

    private InputSystem_Actions input;
    private Coroutine moveCoroutine;
    private bool isMoving;
    private float exitReachDistance = 1.5f;
    private Vector3 originalScale;


    void Awake()
    {
        input = new InputSystem_Actions();
        originalScale = transform.localScale;
    }

    void Start()
    {
        currentNode = AStarManager.instance.FindNearestNode(transform.position);
        // victoryUI.SetActive(false);
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

    public void SetInputPlayerStatus(bool status)
    {
        if (status == false)
        {
            input.Player.Disable();
        } else if (status == true)
        {
            input.Player.Enable();
        }
    }

    void TryMove(Vector3 dir, float yRotation)
    {
        if (isMoving) return;
        if (turnManager && !turnManager.InputEnabled) return;

        Vector3 targetPos = transform.position + dir * tileSize;
        bool isJumping = false;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, out RaycastHit hit, tileSize, barrierMask))
        {
            if (Physics.Raycast(targetPos + Vector3.up, Vector3.down, 2f, groundMask))
            {
                isJumping = true; 
            }
            else return;
        }
        else
        {
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

        while (Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        Vector3 startPos = transform.position;
        float elapsedTime = 0f;
        float duration = Vector3.Distance(startPos, targetPos) / moveSpeed;

        if (audioSource != null)
        audioSource.PlayOneShot(isJumping ? jumpSfx : hopSfx);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float percent = Mathf.Clamp01(elapsedTime / duration);

            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, percent);
            float currentArcHeight = isJumping ? jumpHeight : hopHeight;
            float arc = currentArcHeight * Mathf.Sin(percent * Mathf.PI);
            currentPos.y += arc;
            transform.position = currentPos;

            float stretchY = originalScale.y + (Mathf.Sin(percent * Mathf.PI) * (stretchAmount - 1f));
            float stretchXZ = originalScale.x - (Mathf.Sin(percent * Mathf.PI) * 0.1f);
            transform.localScale = new Vector3(stretchXZ, stretchY, stretchXZ);

            yield return null;
        }

        transform.position = targetPos;
        currentNode = AStarManager.instance.FindNearestNode(transform.position);

        if (audioSource != null && ribbitSfx != null)
        audioSource.PlayOneShot(ribbitSfx);

        yield return StartCoroutine(SquashRoutine());

        isMoving = false;

        if (currentNode == victoryNode) 
        {
            yield return StartCoroutine(AutoMoveToExit());
            yield break; 
        }

        if (turnManager)
            turnManager.EndPlayerTurn();
    }

    IEnumerator AutoMoveToExit()
    {
        Collider[] exits = Physics.OverlapSphere(transform.position, exitReachDistance, exitMask);
        if (exits.Length == 0) yield break;
        
        Vector3 exitPos = exits[0].transform.position;
        exitPos.y = transform.position.y; 

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

        Vector3 startPos = transform.position;
        float walkDuration = 0.5f; 
        float elapsed = 0f;

        while (elapsed < walkDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / walkDuration;
            
            Vector3 currentPos = Vector3.Lerp(startPos, exitPos, percent);
            currentPos.y += hopHeight * Mathf.Sin(percent * Mathf.PI);
            transform.position = currentPos;

            float stretchY = originalScale.y + (Mathf.Sin(percent * Mathf.PI) * (stretchAmount - 1f));
            transform.localScale = new Vector3(originalScale.x, stretchY, originalScale.z);
            
            yield return null;
        }

        transform.position = exitPos;
        yield return StartCoroutine(SquashRoutine());

        input.Player.Disable();
    }
    IEnumerator SquashRoutine()
    {
        float timer = 0f;
        while (timer < squashDuration)
        {
            timer += Time.deltaTime;
            float t = timer / squashDuration;
            float currentY = Mathf.Lerp(originalScale.y * squashAmount, originalScale.y, t);
            // Counter-stretch X and Z to keep volume consistent
            float currentXZ = Mathf.Lerp(originalScale.x * stretchAmount, originalScale.x, t);
            transform.localScale = new Vector3(currentXZ, currentY, currentXZ);
            yield return null;
        }
        transform.localScale = originalScale;
    }
}