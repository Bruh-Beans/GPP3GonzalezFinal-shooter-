using UnityEngine;
using UnityEngine.SceneManagement;

public class Snacks : MonoBehaviour
{
    public GameObject pausePanel; // Reference to the Pause Menu Panel
    private bool isPaused = false; // Keeps track of whether the game is paused

    // Start is called before the first frame update
    void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false); // Ensure the pause panel is hidden initially
        }

        // Lock the cursor at the start of the game
        LockCursor(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Check for the "Escape" key (to pause/unpause the game)
        {
            TogglePause();
        }
    }

    // Toggle the pause state
    public void TogglePause()
    {
        isPaused = !isPaused; // Toggle the paused state
        if (isPaused)
        {
            Time.timeScale = 0; // Pause the game by setting the time scale to 0
            pausePanel.SetActive(true); // Show the pause panel
            LockCursor(false); // Unlock the cursor while the pause menu is active
        }
        else
        {
            Time.timeScale = 1; // Unpause the game by resetting the time scale
            pausePanel.SetActive(false); // Hide the pause panel
            LockCursor(true); // Lock the cursor back after pausing
        }
    }

    // Method to resume the game (attached to the "Resume" button in the pause menu)
    public void ResumeGame()
    {
        TogglePause(); // Toggle the pause state (this will unpause the game)
    }

    // Method to load the main menu (attached to the "Main Menu" button in the pause menu)
    public void ReturnToMainMenu()
    {
        ClosePauseMenu(); // Ensure the pause menu is hidden
        Time.timeScale = 1; // Ensure the game is unpaused when returning to the main menu
        LockCursor(true); // Lock the cursor when returning to the main menu
        SceneManager.LoadScene(0); // Load the main menu scene (index 0)
    }

    // Method to restart the game (attached to the "Restart" button in the pause menu)
    public void RestartGame()
    {
        ClosePauseMenu(); // Ensure the pause menu is hidden
        Time.timeScale = 1; // Ensure the game is unpaused when restarting
        LockCursor(true); // Lock the cursor when restarting the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene
    }

    // Hides the pause menu
    private void ClosePauseMenu()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false); // Deactivate the pause menu
        }
    }

    // Lock or unlock the cursor
    private void LockCursor(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
            Cursor.visible = false; // Hide the cursor
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Make the cursor visible
        }
    }

    // Reset all game-related data, including timer and player progress
    private void ResetGameState()
    {
        PlayerPrefs.DeleteAll(); // Clear all stored player progress or high scores
    }

    // Method to quit the game (optional, attached to a Quit button in the pause menu)
    public void QuitGame()
    {
        Application.Quit(); // Quit the game
    }
}
