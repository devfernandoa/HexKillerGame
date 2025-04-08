using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void OnQuitButtonClicked()
    {
        Debug.Log("Quit button clicked");
        Application.Quit();
    }
}