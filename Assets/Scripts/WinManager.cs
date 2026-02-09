using UnityEngine;

public class WinManager : MonoBehaviour
{
    public Animator fadeInController;
    public int currentLevelNumber = 1;

    private int fadeTriggerHash;
    private bool hasPlayed = false;

    void Awake()
    {
        fadeTriggerHash = Animator.StringToHash("fade");
    }

    public void PlayFadeAnimation()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        if (fadeInController == null)
        {
            Debug.LogWarning("Fade Animator not assigned!");
            return;
        }

        fadeInController.gameObject.SetActive(true);
        fadeInController.SetTrigger(fadeTriggerHash);
        Debug.Log("Fade animation triggered!");

        // Unlock the next level automatically
        MenuManager.instance.UnlockNextLevel(currentLevelNumber);
    }

    private void UnlockNextLevel()
    {
        int nextLevel = currentLevelNumber + 1;

        // Assuming your max level is 3
        if (nextLevel > 3) return;

        PlayerPrefs.SetInt("Level" + nextLevel + "Unlocked", 1);
        PlayerPrefs.Save();

        Debug.Log("Level " + nextLevel + " unlocked!");
    }
}
