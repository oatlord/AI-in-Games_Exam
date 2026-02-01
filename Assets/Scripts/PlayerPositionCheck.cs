using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionCheck : MonoBehaviour
{
    private Ray checkPositionRay;
    private Transform currentTile;
    public float rayLength = 3;
    public LayerMask groundLayer;

    // Update is called once per frame
    void Update()
    {
        checkPositionRay = new Ray(transform.position, Vector3.down * rayLength);

        if (Physics.Raycast(checkPositionRay, out RaycastHit hit, groundLayer))
        {
            currentTile = hit.transform;
            Debug.Log("Player Tile: " + currentTile);

            if (hit.transform.CompareTag("WinTile"))
            {
                Debug.Log("Yay we won");
            }
        }
    }

    public Transform GetCurrentTile()
    {
        return currentTile;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * rayLength);
    }
}
