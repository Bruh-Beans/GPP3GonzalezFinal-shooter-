using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // For UI elements

public class BlockSpawner : MonoBehaviour
{
    public GameObject blockPrefab;
    public float spawnRange = 50f; // range for X and Z
    private GameObject currentBlock;
    private float spawnTimer;
    private float destroyTimer;
    private bool timerRunning;

    // Game Over Image and Retry Button References
    public GameObject gameOverImage;  // Reference to the Game Over Image (instead of Canvas)
    public Button retryButton;        // Reference to the Retry Button

    void Start()
    {
        gameOverImage.SetActive(false);  // Hide the Game Over Image initially
        retryButton.onClick.AddListener(RetryGame);  // Add listener for the retry button
    }

    void Update()
    {
        // Check for the spawn timer
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= 10f && currentBlock == null)
        {
            SpawnBlock();
            spawnTimer = 0f;  // Reset spawn timer after spawning
        }

        // Check for the destroy timer
        if (timerRunning)
        {
            destroyTimer += Time.deltaTime;
            if (destroyTimer >= 10f && currentBlock != null)
            {
                EndGame();  // Trigger the Game Over if the block is not destroyed in time
            }
        }

        if (currentBlock == null && timerRunning)
        {
            timerRunning = false;  // Stop the timer if the block is destroyed
            destroyTimer = 0f;  // Reset destroy timer
        }
    }

    void SpawnBlock()
    {
        // Generate a random position within the spawn range, ensuring it's at height Y = 60
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnRange, spawnRange),
            60f,
            Random.Range(-spawnRange, spawnRange)
        );

        currentBlock = Instantiate(blockPrefab, randomPosition, Quaternion.identity);
        timerRunning = true;  // Start the timer when a block is spawned
        destroyTimer = 0f;   // Reset the destroy timer when a block is spawned
    }

    void EndGame()
    {
        // Pause the game when the game ends
        Time.timeScale = 0f;

        // Show the cursor and enable the Game Over UI (Image)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Activate Game Over Image
        gameOverImage.SetActive(true);
    }

    public void RetryGame()
    {
        // Reset the game by reloading Scene 0
        SceneManager.LoadScene(0);  // Loads the scene with index 0 (make sure Scene 0 is added in Build Settings)

        // Reset game settings to their initial state
        ResetGameSettings();
    }

    // This method resets all the game settings when a retry occurs
    void ResetGameSettings()
    {
        // Reset time scale to normal speed
        Time.timeScale = 1f;

        // Reset cursor state to locked and invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Deactivate the Game Over Image
        gameOverImage.SetActive(false);

        // Reset all spawn timers and states
        spawnTimer = 0f;
        destroyTimer = 0f;
        timerRunning = false;

        // Destroy any remaining block from previous game session
        if (currentBlock != null)
        {
            Destroy(currentBlock);
        }

        // Ensure the block spawns again
        SpawnBlock();
    }
}
