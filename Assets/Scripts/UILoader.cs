using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoader : MonoBehaviour
{
    // public PlayerMovement player;
    private void Update()
    {
        // For pause screen
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     player.SetInputPlayerStatus(false);
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
