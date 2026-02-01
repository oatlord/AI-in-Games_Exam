using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoseManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;

    private PlayerPositionCheck playerPosCheckScript;
    private LoseBehavior playerLoseBehavior;
    private Coroutine playerRespawnCoroutine;
    private bool playerLost = false;

    private AIPositionCheck enemyPosCheckScript;

    private Transform playerCurrentPos;
    private Transform enemyCurrentPos;
    // Start is called before the first frame update
    void Start()
    {
        playerPosCheckScript = player.gameObject.GetComponent<PlayerPositionCheck>();
        enemyPosCheckScript = enemy.gameObject.GetComponent<AIPositionCheck>();

        playerLoseBehavior = player.gameObject.GetComponent<LoseBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        playerCurrentPos = playerPosCheckScript.GetCurrentTile();
        enemyCurrentPos = enemyPosCheckScript.GetCurrentTile();

        if (playerCurrentPos.Equals(enemyCurrentPos))
        {
            Debug.Log("Enemy in player's tile. Game over.");
            playerLost = true;
        }

        if (playerLost == true)
        {
            if (playerRespawnCoroutine == null)
            {
                playerRespawnCoroutine = StartCoroutine(playerLoseBehavior.RespawnBehavior());
            } else if (playerRespawnCoroutine != null)
            {
                StopCoroutine(playerLoseBehavior.RespawnBehavior());
                playerRespawnCoroutine = StartCoroutine(playerLoseBehavior.RespawnBehavior());
            }
        }
    }
}
