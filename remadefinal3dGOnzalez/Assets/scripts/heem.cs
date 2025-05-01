using UnityEngine;
using UnityEngine.SceneManagement;

public class heem : MonoBehaviour
{
    private static heem instance;  // Singleton instance

    private void Awake()
    {
        // Ensure there's only one instance of this script
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Keep this object between scenes
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances
        }
    }

    void Start()
    {
        // Register for the scene load event to listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)  // Check if the current scene is Scene 0
        {
            // Reset everything to its original state when returning to Scene 0
            ResetGameState();
        }
    }

    private void ResetGameState()
    {
        // Reset game-related objects and states back to their defaults when Scene 0 is loaded
        // For example, reset time scale, UI elements, player positions, etc.
        Time.timeScale = 1f;  // Ensure the game is unpaused
        Cursor.lockState = CursorLockMode.Locked;  // Hide the cursor and lock it
        Cursor.visible = false;

        // Add any other reset actions, such as resetting scores, health, or game objects
        Debug.Log("Game state reset to original state.");
    }

    // Optional: This can be called manually if needed to disable or reset the script in other scenes
    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event when the object is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
