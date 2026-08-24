using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    public Vector3 teleportPosition;   // Custom coordinates where the object reappears
    public HUDManager HUDManager;      // Reference to the HUD manager

    void OnTriggerEnter(Collider other)
    {
        // Teleport whatever entered to the custom coordinates
        other.transform.position = teleportPosition;

        // Reset velocity so it moves cleanly from the new spot (if it has physics)
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Update HUD
        if (HUDManager != null)
            HUDManager.IncrementScore();
    }
}