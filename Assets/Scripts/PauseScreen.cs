using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public PlayerMovement player;
    public Animator pauseScreenAnimator;
    public float transitionTime = 1f;
    private bool windowActive = false;
    private bool gamePause = false;
    // public readonly int isActiveHash = Animator.StringToHash("isActive");
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (windowActive == false)
            {
                player.SetInputPlayerStatus(false);
                windowActive = true;
                gamePause = true;
                StartCoroutine(PauseFade("FadeIn"));
            } else
            {
                player.SetInputPlayerStatus(true);
                windowActive = false;
                gamePause = false;
                StartCoroutine(PauseFade("FadeOut"));
            }
        }
    }

    void PauseGame()
    {
        if (gamePause == true)
        {
            Time.timeScale = 0f;
        } else if (gamePause == false)
        {
            Time.timeScale = 1f;
        }
    }

    IEnumerator PauseFade(string triggerName)
    {
        if (pauseScreenAnimator != null)
        {
            pauseScreenAnimator.SetTrigger(triggerName);
        }
        yield return new WaitForSecondsRealtime(transitionTime);
        PauseGame();
    }
}
