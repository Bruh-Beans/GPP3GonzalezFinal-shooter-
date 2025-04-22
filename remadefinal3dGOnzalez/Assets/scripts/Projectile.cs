using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Destroy(gameObject, lifetime); // still destroy after time
    }

    void OnCollisionEnter(Collision collision)
    {
       

        Destroy(gameObject); // destroy the projectile after first hit
    }
}
