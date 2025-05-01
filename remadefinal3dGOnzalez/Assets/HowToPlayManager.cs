using UnityEngine;

public class HowToPlayManager : MonoBehaviour
{
    public GameObject howToPlayPanel; // Reference to the "How to Play" panel

    private void Start()
    {
        // Ensure the panel starts as inactive
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    public void ShowHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true); // Activate the panel
        }
    }

    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false); // Deactivate the panel
        }
    }
}
