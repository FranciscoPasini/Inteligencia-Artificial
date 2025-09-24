using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("Level");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
