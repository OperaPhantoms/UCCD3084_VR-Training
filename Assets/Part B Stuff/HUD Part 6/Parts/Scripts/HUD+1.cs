using UnityEngine;

public class ScoreIncrementTrigger : MonoBehaviour
{
    public HUDManager HUDManager;  // Reference to the HUD manager

    void OnTriggerEnter(Collider other)
    {
        // +1
        HUDManager.IncrementScore();
    }
}