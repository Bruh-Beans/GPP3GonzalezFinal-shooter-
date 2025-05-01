using UnityEngine;

public class DestroyOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject); // destroy the object that touched
        Destroy(gameObject);       // destroy this block
    }
}
