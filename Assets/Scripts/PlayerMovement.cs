using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    public float tileSize = 1f;

    public LayerMask groundMask;
    public LayerMask barrierMask;

    public TurnManager turnManager;

    private InputSystem_Actions input;
    private Coroutine moveCoroutine;
    private bool isMoving;

    void Awake()
    {
        input = new InputSystem_Actions();
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

    // ===============================
    // CORE LOGIC
    // ===============================

    void TryMove(Vector3 dir, float yRotation)
    {
        if (isMoving) return;
        if (turnManager && !turnManager.InputEnabled) return;

        Vector3 targetPos = transform.position + dir * tileSize;

        // Barrier check
        if (Physics.Raycast(transform.position, dir, tileSize, barrierMask))
            return;

        // Ground check
        if (!Physics.Raycast(targetPos + Vector3.up, Vector3.down, 2f, groundMask))
            return;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveRoutine(targetPos, yRotation));
    }

    IEnumerator MoveRoutine(Vector3 targetPos, float yRotation)
    {
        isMoving = true;

        Quaternion targetRot = Quaternion.Euler(0, yRotation, 0);

        // Rotate
        while (Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Move 1 TILE
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;

        if (turnManager)
            turnManager.EndPlayerTurn();
    }
}
