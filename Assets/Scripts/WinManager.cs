using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    private PlayerPositionCheck playerPosCheckScript;
    public GameObject player;
    public GameObject victoryUI;
    public Animator fadeInController;
    private int onActiveAnimationTriggerHash;

    // Start is called before the first frame update
    void Start()
    {
        playerPosCheckScript = player.GetComponent<PlayerPositionCheck>();
        victoryUI.SetActive(false);
        onActiveAnimationTriggerHash = Animator.StringToHash("isActive");

        // // warn early if references aren't assigned in inspector
        // if (victoryUI == null) Debug.LogWarning("[WinManager] victoryUI is not assigned in the Inspector.");
        // if (fadeInController == null) Debug.LogWarning("[WinManager] fadeInController is not assigned in the Inspector.");

        // // Runtime diagnostic: show which controller and parameters are available (helps find mismatches)
        // if (fadeInController != null)
        // {
        //     var rc = fadeInController.runtimeAnimatorController;
        //     Debug.Log($"[WinManager] Assigned Animator.runtimeAnimatorController = {(rc != null ? rc.name : "<null>")}");
        //     // DebugLogAnimatorParameters();
        // }
    }

    // Update is called once per frame
    void Update()
    {
        // Shortcut for victory screen view
        if (Input.GetKeyDown(KeyCode.Q) && victoryUI != null)
        {
            if (victoryUI.activeSelf == false) {
                victoryUI.SetActive(true);
            }
            else {
                victoryUI.SetActive(false);
            }
        }

        if (fadeInController != null)
        {
            Debug.Log("Controller exists");
        }

        Debug.Log(onActiveAnimationTriggerHash);
        Debug.Log("Parameters:" + fadeInController.parameterCount);
        Debug.Log("Animation Controller: " + fadeInController.name);
        Debug.Log(fadeInController.GetParameter(onActiveAnimationTriggerHash));
    }

    // IEnumerator PlayFadeIn() {

    // }
}
