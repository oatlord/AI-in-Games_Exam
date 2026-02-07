using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;
    public Animator transitionAnimator;
    public WinManager winManager;
    public float transitionTime = 1f;

    private bool isLoading = false;

    private void Awake()
    {
        if (instance == null) instance = this;

        if (transitionAnimator != null)
    {
        transitionAnimator.Play("Crossfade_End", 0, 0f);
    }
    }

    void Update()
    {
        // Manual Reset shortcut
        if (Input.GetKeyDown(KeyCode.F) && !isLoading)
        {
            RestartLevel();
        }
    }

    public void BackToMenu()
{
    if (isLoading) return;

    UndoManager.instance?.ResetUndoCount();
    StartCoroutine(LoadMenu());
}

    public void RestartLevel()
    {
        UndoManager.instance?.ResetUndoCount();
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadNextLevel()
    {
        UndoManager.instance?.ResetUndoCount();
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

        IEnumerator LoadMenu()
    {
        isLoading = true;

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start");
        }

        yield return new WaitForSecondsRealtime(transitionTime);

        SceneManager.LoadScene("Menu");
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        isLoading = true;

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start");
        }

        yield return new WaitForSecondsRealtime(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}