using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int totalKillCount = 0;
    public int killCombo = 0;

    public TextMeshProUGUI totalKillText;
    public TextMeshProUGUI comboKillText;

    // New function to reset total kills, if needed
    public void ResetTotalKills()
    {
        totalKillCount = 0;
        UpdateKillUI();  // Update the UI after reset
    }

    private void Awake()
    {
        // Singleton setup
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
        totalKillCount += amount;
        killCombo += amount;
        UpdateKillUI();
    }

    public void ResetKillCombo()
    {
        killCombo = 0;
        UpdateKillUI();
    }

    void UpdateKillUI()
    {
        if (totalKillText != null)
        {
            totalKillText.text = "Total Kills: " + totalKillCount.ToString();
        }

        if (comboKillText != null)
        {
            comboKillText.text = "Kill Combo: " + killCombo.ToString();
        }
    }
}
