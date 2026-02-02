using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;

    private bool isPlayerTurn = true;
    private bool isEnemyTurn = false;

    private PlayerMovement playerScript;
    private AIMovement enemyScript;

    // 0 = player, 1 = enemy (kept for compatibility)
    private int turnState;

    public float switchDelay = 0.15f;
    public float simulatedEnemyMoveDuration = 1f;

    // gate player input while switching / enemy thinking
    private bool inputEnabled = true;
    public bool InputEnabled => inputEnabled;

    void Start()
    {
        turnState = 0;
        isPlayerTurn = true;
        isEnemyTurn = false;

        if (player != null) {
            playerScript = player.gameObject.GetComponent<PlayerMovement>();
            playerScript.enabled = true;
        } 
        if (enemy != null) {
            enemyScript = enemy.gameObject.GetComponent<AIMovement>();
            enemyScript.enabled = false;
        }
        inputEnabled = true;
    }

    void Update()
    {
        // keep visual state consistent (useful when toggling in editor)
        if (player != null && enemy != null)
        {
            if (isPlayerTurn && !isEnemyTurn)
            {
                playerScript.enabled = true;
                enemyScript.enabled = false;
            }
            else if (!isPlayerTurn && isEnemyTurn)
            {
                playerScript.enabled = false;
                enemyScript.enabled = true;
            }
        }
    }

    // Call this when the player finishes their move
    public void EndPlayerTurn()
    {
        if (!isPlayerTurn || !inputEnabled) return; 

        isPlayerTurn = false;
        isEnemyTurn = true;
        turnState = 1;
        StartCoroutine(TurnSwitching());
    }

    IEnumerator TurnSwitching()
    {
        // disable player input while switching / enemy thinking
        inputEnabled = false;

        if (player != null) playerScript.enabled = isPlayerTurn;
        if (enemy != null) enemyScript.enabled = isEnemyTurn;

        yield return new WaitForSeconds(switchDelay);

        if (isEnemyTurn && enemy != null)
        {
            // if the enemy has an AIMovement with a TakeTurn coroutine, run it
            var ai = enemy.GetComponent<AIMovement>();
            if (ai != null)
            {
                yield return StartCoroutine(ai.TakeTurn());
            }
            else
            {
                // fallback: simulate a short AI action
                yield return new WaitForSeconds(simulatedEnemyMoveDuration);
            }

            // after enemy completes, switch back to player
            isEnemyTurn = false;
            isPlayerTurn = true;
            turnState = 0;
            if (player != null && playerScript != null) playerScript.enabled = true;
            if (enemy != null && enemyScript != null) enemyScript.enabled = false;
        }

        inputEnabled = true;
    }
}
