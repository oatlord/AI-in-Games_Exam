using System.Collections;
using UnityEngine;

public class LoseBehavior : MonoBehaviour
{
    public Transform respawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator RespawnBehavior()
    {
        Debug.Log("Player lost. Respawning to original position.");
        yield return new WaitForSeconds(2F);
        transform.position = new Vector3(respawnPoint.position.x, respawnPoint.position.y, respawnPoint.position.z);
    }
}
