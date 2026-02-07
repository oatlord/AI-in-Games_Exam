using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public CanvasGroup mainMenuPanel;
    public CanvasGroup settingsPanel;
    public CanvasGroup selectionPanel;
    public CanvasGroup levelSelectionPanel;

    [Header("Level Buttons")]
    public Button level2Button;
    public Button level3Button;

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
        SceneManager.LoadScene("TutorialScene");
    }

    public void OpenLevelSelection()
    {
        ShowPanel(levelSelectionPanel);
        HidePanel(selectionPanel);
    }

    // LEVEL SELECTION
    public void CloseLevelSelection()
    {
        ShowPanel(selectionPanel);
        HidePanel(levelSelectionPanel);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level3");
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
