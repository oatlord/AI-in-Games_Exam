using UnityEngine;

public class WinManager : MonoBehaviour
{
    public Animator fadeInController;

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
    }

}
