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
            transitionAnimator.Play("Crossfade_End", 0, 0f);

        // --- DEFAULT LEVEL UNLOCKS ---
        if (!PlayerPrefs.HasKey("FirstRun"))
        {
            Debug.Log("First run detected. Setting default level locks.");
            PlayerPrefs.SetInt("Level1Unlocked", 1); // Level 1 unlocked by default
            PlayerPrefs.SetInt("Level2Unlocked", 0); // Level 2 locked
            PlayerPrefs.SetInt("Level3Unlocked", 0); // Level 3 locked
            PlayerPrefs.SetInt("FirstRun", 1);       // Mark first run complete
            PlayerPrefs.Save();
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

    #region Button Methods
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
        UpdateLevelLocks(); // refresh button states
        ShowPanel(levelSelectionPanel);
        HidePanel(selectionPanel);
    }

    public void CloseLevelSelection()
    {
        ShowPanel(selectionPanel);
        HidePanel(levelSelectionPanel);
    }

    public void LoadLevel1() => StartCoroutine(LoadLevelRoutine("Level 1"));
    public void LoadLevel2() => StartCoroutine(LoadLevelRoutine("Level 2"));
    public void LoadLevel3() => StartCoroutine(LoadLevelRoutine("Level 3"));
    public void ReturnToMenu() => StartCoroutine(LoadLevelRoutine("Menu"));
    #endregion

    private IEnumerator LoadLevelRoutine(string sceneName)
    {
        if (transitionAnimator != null)
            transitionAnimator.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(transitionTime);

        SceneManager.LoadScene(sceneName);
    }

    #region Level Unlock System
    public void UnlockNextLevel(int completedLevel)
    {
        int nextLevel = completedLevel + 1;
        if (nextLevel > 3) return; // no level beyond 3

        PlayerPrefs.SetInt("Level" + nextLevel + "Unlocked", 1);
        PlayerPrefs.Save();

        Debug.Log("Level " + nextLevel + " unlocked!");
    }

    private void UpdateLevelLocks()
    {
        level2Button.interactable = PlayerPrefs.GetInt("Level2Unlocked", 0) == 1;
        level3Button.interactable = PlayerPrefs.GetInt("Level3Unlocked", 0) == 1;
    }
    #endregion

    #region UI Helpers
    private void ShowPanel(CanvasGroup panel)
    {
        panel.alpha = 1;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    private void HidePanel(CanvasGroup panel)
    {
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }
    #endregion
}
