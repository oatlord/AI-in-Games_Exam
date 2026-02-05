using UnityEngine;

public class PlayerPositionCheck : MonoBehaviour
{
    public Transform currentTile;
    public float rayLength = 3f;

    [Header("Detection Layers")]
    public LayerMask groundLayer;
    public LayerMask exitLayer;

    private LayerMask combinedMask;
    private bool hasWon = false;

    public WinManager winManager;

    void Start()
    {
        // Combine Ground + Exit
        combinedMask = groundLayer | exitLayer;
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayLength, combinedMask))
        {
            currentTile = hit.transform;

            if (!hasWon && hit.transform.gameObject.layer == LayerMask.NameToLayer("Exit"))
            {
                hasWon = true;
                Debug.Log("EXIT REACHED");
                winManager.PlayFadeAnimation();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * rayLength);
    }
}
