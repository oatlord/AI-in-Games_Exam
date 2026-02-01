using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPositionCheck : MonoBehaviour
{
    private Ray positionCheckRay;
    private Transform currentTile;

    public float rayLength = 3;
    public LayerMask groundMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        positionCheckRay = new Ray(transform.position, Vector3.down * rayLength);

        if (Physics.Raycast(positionCheckRay, out RaycastHit hit, groundMask))
        {
            Debug.Log("Enemy Tile: " + currentTile);
            currentTile = hit.transform;
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
