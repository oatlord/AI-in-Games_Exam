using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionCheck : MonoBehaviour
{
    private Ray checkPositionRay;
    private Transform currentTile;
    public float rayLength = 3;
    public LayerMask groundLayer;

    private Transform lastTile; // to track the previous tile

    void Update()
    {
        checkPositionRay = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(checkPositionRay, out RaycastHit hit, rayLength, groundLayer))
        {
            // Only log if we moved to a new tile
            if (hit.transform != lastTile)
            {
                currentTile = hit.transform;
                Debug.Log("Player Tile: " + currentTile);

                if (currentTile.CompareTag("WinTile"))
                {
                    Debug.Log("Yay we won");
                }

                lastTile = currentTile; // update lastTile
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
