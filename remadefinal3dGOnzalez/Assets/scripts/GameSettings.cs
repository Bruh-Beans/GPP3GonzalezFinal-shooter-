using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public float deerSightRange = 35f;
    public float deerDetectRadius = 10f;
    public float deerBaseSpeed = 6f;
    public float deerRunSpeed = 25f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDifficulty(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                deerSightRange = 15f;
                deerDetectRadius = 6f;
                deerBaseSpeed = 4f;
                deerRunSpeed = 15f;
                break;
            case "Hard":
                deerSightRange = 70f;
                deerDetectRadius = 15f;
                deerBaseSpeed = 40f;
                deerRunSpeed = 70f;
                break;
            case "SuperHard":
                deerSightRange = 140f;
                deerDetectRadius = 70f;
                deerBaseSpeed = 150f;
                deerRunSpeed = 250f;
                break;
        }
    }
}
