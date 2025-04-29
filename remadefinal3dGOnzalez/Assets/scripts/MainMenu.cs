using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Call this to start the game
    public void PlayGame()
    {
        // Assumes the next scene in build index is the game scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Call this to quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    // Optional: Add settings function here
    public void OpenSettings()
    {
        Debug.Log("Settings menu opened.");
        // You can enable a settings panel here if you have one
    }
}

