using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// [RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // private CharacterController controller;
    public float speed = 3.0f;
    public float rotationSpeed = 180f; // degrees per second
    public LayerMask groundMask;
    public bool moveFinished = false;

    private InputSystem_Actions inputActions;

    private Ray floorRay;
    private Ray pathRay;                                

    private Quaternion targetRotation;

    private Coroutine moveAndRotateCoroutine;
    public TurnManager turnManager;
    // Start is called before the first frame update

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Start()
    {
        targetRotation = transform.rotation;
        inputActions.Player.MoveUp.performed += OnMoveUp;
        inputActions.Player.MoveDown.performed += OnMoveDown;
        inputActions.Player.MoveLeft.performed += OnMoveLeft;
        inputActions.Player.MoveRight.performed += OnMoveRight;

        if (turnManager == null)
        {
            turnManager = FindObjectOfType<TurnManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayOriginLocal = new Vector3(0, 0, 2); // offset 2 units forward in local space
        Vector3 rayOriginWorld = transform.TransformPoint(rayOriginLocal);
        floorRay = new Ray(rayOriginWorld, -Vector3.up * 3); // direction stays world-down
        pathRay = new Ray(transform.position, gameObject.transform.forward * 3);

        Debug.Log("Move finished: " + moveFinished);
    }

    void OnMoveUp(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            if (turnManager != null && !turnManager.InputEnabled) return;
            Debug.Log("Move performed");
            targetRotation = Quaternion.Euler(0, 0, 0);

            if (moveAndRotateCoroutine == null) {
                moveAndRotateCoroutine = StartCoroutine(MovePlayerCoroutine());
            } else if (moveAndRotateCoroutine != null) {
                StopCoroutine(moveAndRotateCoroutine);
                moveAndRotateCoroutine = StartCoroutine(MovePlayerCoroutine());
            }
        }
    }

    void OnMoveDown(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            if (turnManager != null && !turnManager.InputEnabled) return;
            Debug.Log("Move performed");
            targetRotation = Quaternion.Euler(0, 180, 0);

            if (moveAndRotateCoroutine == null) {
                moveAndRotateCoroutine = StartCoroutine(MovePlayerCoroutine());
            } else if (moveAndRotateCoroutine != null) {
                StopCoroutine(moveAndRotateCoroutine);
                moveAndRotateCoroutine = StartCoroutine(MovePlayerCoroutine());
            }
        }
    }

    void OnMoveLeft(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            if (turnManager != null && !turnManager.InputEnabled) return;
            Debug.Log("Move performed");
            targetRotation = Quaternion.Euler(0, -90, 0);

            if (moveAndRotateCoroutine == null) {
                moveAndRotateCoroutine = StartCoroutine(MovePlayerCoroutine());
            } else if (moveAndRotateCoroutine != null) {
                StopCoroutine(moveAndRotateCoroutine);
                moveAndRotateCoroutine = StartCoroutine(MovePlayerCoroutine());
            }
        }
    }

    void OnMoveRight(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            if (turnManager != null && !turnManager.InputEnabled) return;
            Debug.Log("Move performed");
            targetRotation = Quaternion.Euler(0, 90, 0);

            if (moveAndRotateCoroutine == null) {
                moveAndRotateCoroutine = StartCoroutine(MovePlayerCoroutine());
            } else if (moveAndRotateCoroutine != null) {
                StopCoroutine(moveAndRotateCoroutine);
                moveAndRotateCoroutine = StartCoroutine(MovePlayerCoroutine());
            }
        }
    }

    void Move(Vector3 newPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, newPosition, newPosition.magnitude * speed * Time.deltaTime);
    }

    public Vector3 GetNextBlock()
    {
        if (Physics.Raycast(floorRay, out RaycastHit hit, groundMask))
        {
            Debug.Log("Floor Hit: " + hit.collider.name);
            Vector3 newPosition = new Vector3(hit.transform.position.x, hit.transform.position.y + 1, hit.transform.position.z);
            return newPosition;
        }
        else
        {
            return transform.position;
        }
    }

    IEnumerator MovePlayerCoroutine()
    {
        moveFinished = false;
        
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        
        Vector3 nextBlock = GetNextBlock();
        while (Vector3.Distance(transform.position, nextBlock) > 0.01f)
        {
            Move(nextBlock);
            yield return null;
        }
        
        transform.position = nextBlock;

        yield return new WaitUntil(() => transform.rotation == targetRotation && transform.position == nextBlock);
        moveFinished = true;

        // notify TurnManager that the player finished their move
        if (turnManager != null)
        {
            turnManager.EndPlayerTurn();
        }
    }

    public bool GetMoveFinished()
    {
        return moveFinished;
    }

    private void OnDrawGizmos()
    {
        // Raycast for floor check to get hit block below player
        Vector3 rayOriginLocal = new Vector3(0, 0, 2); // offset 2 unit forward in local space
        Vector3 rayOriginWorld = transform.TransformPoint(rayOriginLocal);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOriginWorld, -Vector3.up * 3);

        // Raycast for forward direction, checking if path is clear
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 3);
    }
}
