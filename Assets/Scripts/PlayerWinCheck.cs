using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWinCheck : MonoBehaviour
{
    private Ray checkPositionRay;
    public float rayLength = 3;
    public LayerMask groundLayer;

    // Update is called once per frame
    void Update()
    {
        checkPositionRay = new Ray(transform.position, Vector3.down * rayLength);

        if (Physics.Raycast(checkPositionRay, out RaycastHit hit, groundLayer))
        {
            if (hit.transform.CompareTag("WinTile"))
            {
                Debug.Log("Yay we won");
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * rayLength);
    }
}
