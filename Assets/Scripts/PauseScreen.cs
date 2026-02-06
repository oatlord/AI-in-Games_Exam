using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    public PlayerMovement player;
    public CanvasGroup pauseGroup;

    private bool isPaused;

    void Start()
    {
        ApplyPauseState(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        ApplyPauseState(!isPaused);
    }

    void ApplyPauseState(bool pause)
    {
        isPaused = pause;

        Time.timeScale = pause ? 0f : 1f;

        player.SetInputPlayerStatus(!pause);

        pauseGroup.alpha = pause ? 1f : 0f;
        pauseGroup.interactable = pause;
        pauseGroup.blocksRaycasts = pause;
    }

    public void ReturnToMenu()
    {
        ApplyPauseState(false);
        SceneManager.LoadScene("Menu");
    }
}
