using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyMenu : MonoBehaviour
{
    public void SetEasy()
    {
        GameSettings.Instance.SetDifficulty("Easy");
        SceneManager.LoadScene("GameScene"); // Replace with your scene name
    }

    public void SetHard()
    {
        GameSettings.Instance.SetDifficulty("Hard");
        SceneManager.LoadScene("GameScene");
    }

    public void SetSuperHard()
    {
        GameSettings.Instance.SetDifficulty("SuperHard");
        SceneManager.LoadScene("GameScene");
    }
}
