using UnityEngine;

public class DeerSpawner : MonoBehaviour
{
    public GameObject deerPrefab;
    public int deerCount = 500;
    public Vector3 spawnArea = new Vector3(200, 0, 200);

    void Start()
    {
        int spawnSuccess = 0;

        for (int i = 0; i < deerCount; i++)
        {
            Vector3 randomPos = transform.position + new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                10f,
                Random.Range(-spawnArea.z, spawnArea.z)
            );

            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 20f))
            {
                Vector3 spawnPos = hit.point;
                Instantiate(deerPrefab, spawnPos, Quaternion.identity);
                spawnSuccess++;
            }
            else
            {
                Debug.LogWarning($"Raycast failed at position: {randomPos}");
            }
        }

        Debug.Log($"Successfully spawned {spawnSuccess} out of {deerCount} deer.");
    }
}
