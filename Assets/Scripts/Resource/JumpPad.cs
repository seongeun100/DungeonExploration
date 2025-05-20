using UnityEngine;

public class JumpPad : MonoBehaviour
{
    float jumpPower = 500f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
            }
        }
    }
}
