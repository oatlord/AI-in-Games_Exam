using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    public Canvas canvas;

    private PlayerPositionCheck playerPosCheckScript;
    private bool playerLost = false;

    private AIPositionCheck enemyPosCheckScript;

    private Scene activeScene;
    private Image transitionImage;
    private Coroutine reloadSceneCoroutine;

    private Transform playerCurrentPos;
    private Transform enemyCurrentPos;
    // Start is called before the first frame update
    void Start()
    {
        playerPosCheckScript = player.gameObject.GetComponent<PlayerPositionCheck>();
        enemyPosCheckScript = enemy.gameObject.GetComponent<AIPositionCheck>();
        activeScene = SceneManager.GetActiveScene();

        transitionImage = canvas.GetComponentInChildren<Image>();
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

        Debug.Log(activeScene.name);

        if (playerLost == true)
        {
            if (reloadSceneCoroutine == null)
            {
                reloadSceneCoroutine = StartCoroutine(Transition());
            }
        }
    }

    IEnumerator Transition() {
        RectTransform rectTransform = transitionImage.GetComponent<RectTransform>();
        
        // Start position: off-screen to the right
        Vector2 startPos = new Vector2(Screen.width, rectTransform.anchoredPosition.y);
        // End position: off-screen to the left
        Vector2 endPos = new Vector2(-Screen.width, rectTransform.anchoredPosition.y);
        
        rectTransform.anchoredPosition = startPos;
        float elapsed = 0f;
        float duration = 5f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);
            
            // Reload scene at halfway point (2.5 seconds)
            if (elapsed >= duration / 2f && reloadSceneCoroutine != null)
            {
                SceneManager.LoadScene(activeScene.name);
                reloadSceneCoroutine = null;
            }
            
            yield return null;
        }
        
        rectTransform.anchoredPosition = endPos;
    }
}
