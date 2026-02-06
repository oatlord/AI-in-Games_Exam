using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoader : MonoBehaviour
{
    // public PlayerMovement player;
    // public GameObject pauseCanvas;
    private void Update()
    {
        // For pause screen
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     player.SetInputPlayerStatus(false);
        // }
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     if (pauseCanvas.activeSelf == false)
        //     {
        //         pauseCanvas.SetActive(true);
        //     } else
        //     {
        //         pauseCanvas.SetActive(false);
        //     }
        // }
    }
    public void ExitGame() {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
