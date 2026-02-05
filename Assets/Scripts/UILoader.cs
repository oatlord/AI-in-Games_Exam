using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoader : MonoBehaviour
{
    public void ExitGame() {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
