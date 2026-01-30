using UnityEngine;

public class AIMovement : MonoBehaviour
{
    private GameObject player;

    private Ray floorRay;
    private Ray pathRay;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayOriginLocal = new Vector3(0, 0, 4); // offset 2 units forward in local space
        Vector3 rayOriginWorld = transform.TransformPoint(rayOriginLocal);
        floorRay = new Ray(rayOriginWorld, -Vector3.up * 3); // direction stays world-down
        pathRay = new Ray(transform.position, gameObject.transform.forward * 3);
    }

    private void OnDrawGizmos()
    {
        // Raycast for floor check to get hit block below player
        Vector3 rayOriginLocal = new Vector3(0, 0, 4); // offset 2 unit forward in local space
        Vector3 rayOriginWorld = transform.TransformPoint(rayOriginLocal);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOriginWorld, -Vector3.up * 3);

        // Raycast for forward direction, checking if path is clear
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 3);
    }
}
