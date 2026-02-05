using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    private PlayerPositionCheck playerPosCheckScript;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        playerPosCheckScript = player.GetComponent<PlayerPositionCheck>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPosCheckScript.GetCurrentTile().CompareTag("WinTile"))
        {
            Debug.Log("Yay we won");
        }
    }
}
