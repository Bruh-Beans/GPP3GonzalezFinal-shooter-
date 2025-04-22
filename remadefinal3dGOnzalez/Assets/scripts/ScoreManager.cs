using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int killCount = 0;
    public TextMeshProUGUI killText;


    private void Awake()
    {
        // Singleton setup so it's easy to access from anywhere
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateKillUI();
    }

    public void IncreaseKillCount(int amount)
    {
        killCount += amount;
        UpdateKillUI();
    }

    void UpdateKillUI()
    {
        if (killText != null)
        {
            killText.text = "Kills: " + killCount.ToString();
        }
    }
}
