using UnityEngine;
using UnityEngine.UI;  // For Slider component

public class BloodThirstManager : MonoBehaviour
{
    public Slider killSlider;  // Reference to the Slider UI
    public float maxKills = 10f;  // Max number of kills needed to fill the slider
    private float currentKills = 0f;  // Current number of kills

    void Start()
    {
        // Initialize the slider values
        if (killSlider != null)
        {
            killSlider.minValue = 0f;
            killSlider.maxValue = maxKills;
            killSlider.value = currentKills;
        }
    }

    // Call this method when a kill happens
    public void OnKill()
    {
        // Increase the kill count
        currentKills += 1f;

        // Make sure the kill count doesn't exceed the max value
        currentKills = Mathf.Clamp(currentKills, 0f, maxKills);

        // Update the slider value to reflect the new number of kills
        if (killSlider != null)
        {
            killSlider.value = currentKills;
        }

        Debug.Log("Current Kills: " + currentKills);
    }
}
