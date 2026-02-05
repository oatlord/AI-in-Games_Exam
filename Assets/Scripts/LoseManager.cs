using System.Collections;
using UnityEngine;

public class LoseManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;

    private PlayerPositionCheck playerPosCheckScript;
    private AIPositionCheck enemyPosCheckScript;
    private bool isRestarting = false;

    void Start()
    {
        playerPosCheckScript = player.GetComponent<PlayerPositionCheck>();
        enemyPosCheckScript = enemy.GetComponent<AIPositionCheck>();
    }

void Update()
{
    if (isRestarting) return;

    if (Input.GetKeyDown(KeyCode.F))
    {
        InitiateReset();
    }

    Transform playerCurrentPos = playerPosCheckScript.currentTile;
    Transform enemyCurrentPos = enemyPosCheckScript.GetCurrentTile();

    if (playerCurrentPos != null && playerCurrentPos.Equals(enemyCurrentPos))
    {
        AIMovement aiScript = enemy.GetComponent<AIMovement>();
        
        if (!aiScript.GetIsTakingTurn()) 
        {
            InitiateReset();
        }
    }
}

void InitiateReset()
{
    if (isRestarting) return;
    isRestarting = true;

    StartCoroutine(ResetSequence());
}
IEnumerator ResetSequence()
{
    Debug.Log("gottem.");
    isRestarting = true;

    AIMovement ai = enemy.GetComponent<AIMovement>();
    
    if (ai.currentNode != null)
    {
        enemy.transform.position = ai.currentNode.transform.position;
    }
    
    Vector3 lockPos = enemy.transform.position;
    Vector3 baseScale = ai.transform.localScale;

    int hops = 0;
    while (hops < 2)
    {
        float elapsed = 0f;
        float duration = 0.4f;

        if (ai.audioSource != null && ai.hopSfx != null)
            ai.audioSource.PlayOneShot(ai.hopSfx);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = Mathf.Clamp01(elapsed / duration);

            float arc = ai.hopHeight * Mathf.Sin(percent * Mathf.PI);
            enemy.transform.position = lockPos + Vector3.up * arc;

            float sY = baseScale.y + (Mathf.Sin(percent * Mathf.PI) * (ai.stretchAmount - 1f));
            float sXZ = baseScale.x - (Mathf.Sin(percent * Mathf.PI) * 0.1f);
            enemy.transform.localScale = new Vector3(sXZ, sY, sXZ);

            yield return null;
        }
        
        float squashTimer = 0f;
        float squashDuration = 0.15f; 
        while (squashTimer < squashDuration)
        {
            squashTimer += Time.deltaTime;
            float t = squashTimer / squashDuration;
            float currentSquashY = Mathf.Lerp(baseScale.y * ai.squashAmount, baseScale.y, t);
            
            enemy.transform.position = lockPos; 
            enemy.transform.localScale = new Vector3(baseScale.x * ai.stretchAmount, currentSquashY, baseScale.z * ai.stretchAmount);
            yield return null;
        }

        enemy.transform.localScale = baseScale;
        hops++;
        yield return new WaitForSeconds(0.1f); 
    }

    if (ai.audioSource != null && ai.ribbitSfx != null)
        ai.audioSource.PlayOneShot(ai.ribbitSfx);

    yield return new WaitForSeconds(0.5f);

    if (UndoManager.instance != null) UndoManager.instance.ResetUndoCount();
    if (LevelLoader.instance != null) LevelLoader.instance.RestartLevel();
}
}