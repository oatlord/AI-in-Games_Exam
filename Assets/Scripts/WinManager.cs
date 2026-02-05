using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    private PlayerPositionCheck playerPosCheckScript;

    [Header("Public References")]
    public GameObject player;
    public GameObject victoryUI;
    public Animator fadeInController;
    public LevelLoader levelLoader;

    [Header("Animator References")]
    private int onActiveAnimationTriggerHash;

    // Start is called before the first frame update
    void Start()
    {
        playerPosCheckScript = player.GetComponent<PlayerPositionCheck>();
        victoryUI.SetActive(false);
        onActiveAnimationTriggerHash = Animator.StringToHash("isActive");
    }

    // Update is called once per frame
    void Update()
    {
        // Shortcut for victory screen view
        if (Input.GetKeyDown(KeyCode.Q) && victoryUI != null)
        {
            if (victoryUI.activeSelf == false)
            {
                victoryUI.SetActive(true);
            }
            else
            {
                victoryUI.SetActive(false);
            }
        }

        if (fadeInController != null)
        {
            Debug.Log("Controller exists");
        }

        // Debug.Log(onActiveAnimationTriggerHash);
        // Debug.Log("Parameters:" + fadeInController.parameterCount);
        // Debug.Log("Animation Controller: " + fadeInController.name);
    }

    // public void NextLevel()
    // {
    //     levelLoader = 
    // }

    public void TurnOnUi()
    {
        if (victoryUI.activeSelf == false)
        {
            victoryUI.SetActive(true);
        }
    }

    public void TurnOffUi()
    {
        if (victoryUI.activeSelf == true)
        {
            victoryUI.SetActive(false);
        }
    }
}
