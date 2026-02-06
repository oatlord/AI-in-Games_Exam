using System.Collections;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public PlayerMovement player;
    public Animator pauseScreenAnimator;
    public GameObject pauseCanvas;
    public float transitionTime = 1f;

    private bool windowActive = false;
    private bool gamePauseActivated = false;

    private Coroutine pauseFadeCoroutine;

    void Update()
    {
        Debug.Log("Pause Canvas active: "+pauseCanvas.activeSelf);
        Debug.Log("Game Pause Activated: "+gamePauseActivated);
    }

    void PauseGame()
    {
        if (gamePauseActivated == false)
        {
            Time.timeScale = 0f;
            gamePauseActivated = true;
            player.SetInputPlayerStatus(false);
            pauseCanvas.SetActive(true);
        } else
        {
            Time.timeScale = 1f;
            gamePauseActivated = false;
            player.SetInputPlayerStatus(true);
            pauseCanvas.SetActive(false);
        }
    }

    public void PauseButton()
    {
        if (pauseFadeCoroutine == null)
        {
            pauseFadeCoroutine = StartCoroutine(PauseFade());
        } else
        {
            StopCoroutine(pauseFadeCoroutine);
            pauseFadeCoroutine = StartCoroutine(PauseFade());
        }
    }

    public void ButtonPress()
    {
        Debug.Log("Button pressed");
    }

    IEnumerator PauseFade()
    {
        if (pauseScreenAnimator != null && pauseScreenAnimator.runtimeAnimatorController != null)
        {
            if (gamePauseActivated == false)
            {
                pauseScreenAnimator.SetTrigger("FadeIn");
                Debug.Log("Game Paused");
            } else
            {
                pauseScreenAnimator.SetTrigger("FadeOut");
                Debug.Log("Game Unpaused");
            }
        }
        yield return new WaitForSecondsRealtime(transitionTime);
        PauseGame();
        // yield break;
    }
}
