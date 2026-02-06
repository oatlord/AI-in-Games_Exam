using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public PlayerMovement player;
    public CanvasGroup pauseGroup;

    private bool windowActive = false;
    private bool gamePause = false;

    void Start()
    {
        SetPauseUI(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            windowActive = !windowActive;
            gamePause = windowActive;

            player.SetInputPlayerStatus(!windowActive);
            PauseGame();
            SetPauseUI(windowActive);
        }
    }

    void PauseGame()
    {
        Time.timeScale = gamePause ? 0f : 1f;
    }

    void SetPauseUI(bool show)
    {
        pauseGroup.alpha = show ? 1f : 0f;
        pauseGroup.interactable = show;
        pauseGroup.blocksRaycasts = show;
    }
}
