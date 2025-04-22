using System.Collections;
using UnityEngine;

public class Melee : MonoBehaviour
{
    private Collider weaponCollider;
    private bool isAttacking = false;

    void Start()
    {
        weaponCollider = GetComponent<Collider>();
        weaponCollider.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(ActivateColliderTemporarily());
        }
    }

    IEnumerator ActivateColliderTemporarily()
    {
        isAttacking = true;
        weaponCollider.enabled = true;
        yield return new WaitForSeconds(1f); // enable for 1 second
        weaponCollider.enabled = false;
        isAttacking = false;
    }
}
