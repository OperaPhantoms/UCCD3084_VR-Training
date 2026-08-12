using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
    [Header("References")]
    public BoxCounterManager counterManager;   // Reference to the counter on the clipboard root

    [Header("Teleport Destination")]
    public Vector3 teleportPosition = new Vector3(0f, 2f, 1f);  // Set this in Inspector

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object has the correct tag
        if (other.CompareTag("Box"))
        {
            Debug.Log("TrackedCube entered trigger: " + other.name);

            // Increment the counter
            if (counterManager != null)
            {
                counterManager.IncrementCount();
                Debug.Log("Counter incremented to: " + counterManager.currentCount);
            }
            else
            {
                Debug.LogWarning("BoxCounterManager reference is missing on BoxTrigger!");
            }

            // *** TELEPORT LOGIC (simplified) ***
            // Teleport the cube to the hardcoded position.
            // This is the part you will later replace with Destroy(other.gameObject);
            other.transform.position = teleportPosition;
            // *** END TELEPORT LOGIC ***
        }
        else
        {
            Debug.Log("Something entered trigger but tag was: " + other.tag);
        }
    }
}