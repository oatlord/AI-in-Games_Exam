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
    }

    void Update()
    {
        // Manual Reset shortcut
        if (Input.GetKeyDown(KeyCode.F) && !isLoading)
        {
            RestartLevel();
        }
    }

    public void RestartLevel()
    {
        UndoManager.instance?.ResetUndoCount();
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadNextLevel()
    {
        UndoManager.instance?.ResetUndoCount();
        winManager.TurnOffUi();
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        isLoading = true;

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start");
        }

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}