using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [Header("Panels")]
    public CanvasGroup mainMenuPanel;
    public CanvasGroup settingsPanel;
    public CanvasGroup selectionPanel;
    public CanvasGroup levelSelectionPanel;
    public Animator transitionAnimator;
    public float transitionTime = 1f;

    [Header("Level Buttons")]
    public Button level2Button;
    public Button level3Button;

    private void Awake()
{
    if (instance == null)
        instance = this;
    else
        Destroy(gameObject);

    if (transitionAnimator != null)
    {
        transitionAnimator.Play("Crossfade_End", 0, 0f);
    }
}


    private void Start()
    {
        ShowPanel(mainMenuPanel);
        HidePanel(settingsPanel);
        HidePanel(selectionPanel);
        HidePanel(levelSelectionPanel);

        UpdateLevelLocks();
    }

    public void StartButton()
    {
        ShowPanel(selectionPanel);
        HidePanel(mainMenuPanel);
    }

    public void SettingsButton()
    {
        ShowPanel(settingsPanel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void CloseSettings()
    {
        HidePanel(settingsPanel);
    }

    public void CloseSelection()
    {
        ShowPanel(mainMenuPanel);
        HidePanel(selectionPanel);
    }

    public void Tutorial()
    {
        StartCoroutine(LoadLevelRoutine("Tutorial"));
    }

    public void OpenLevelSelection()
    {
        ShowPanel(levelSelectionPanel);
        HidePanel(selectionPanel);
    }
    public void CloseLevelSelection()
    {
        ShowPanel(selectionPanel);
        HidePanel(levelSelectionPanel);
    }

    public void LoadLevel1()
    {
        StartCoroutine(LoadLevelRoutine("Level 1"));
    }

    public void LoadLevel2()
    {
        StartCoroutine(LoadLevelRoutine("Level 2"));
    }

    public void LoadLevel3()
    {
        StartCoroutine(LoadLevelRoutine("Level 3"));
    }

    public void ReturnToMenu()
    {
        StartCoroutine(LoadLevelRoutine("Menu"));
    }

    private IEnumerator LoadLevelRoutine(string sceneName)
    {
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start");
        }

        yield return new WaitForSecondsRealtime(transitionTime);

        SceneManager.LoadScene(sceneName);
    }

    // LOCK SYSTEM
    void UpdateLevelLocks()
    {
        bool level2Unlocked = PlayerPrefs.GetInt("Level2Unlocked", 0) == 1;
        bool level3Unlocked = PlayerPrefs.GetInt("Level3Unlocked", 0) == 1;

        level2Button.interactable = level2Unlocked;
        level3Button.interactable = level3Unlocked;
    }

    void ShowPanel(CanvasGroup panel)
    {
        panel.alpha = 1;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    void HidePanel(CanvasGroup panel)
    {
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }
}
