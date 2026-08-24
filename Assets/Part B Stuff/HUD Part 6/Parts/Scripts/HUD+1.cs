using UnityEngine;

public class ScoreIncrementTrigger : MonoBehaviour
{
    public HUDManager HUDManager;  // Reference to the HUD manager
    public ParticleSystem deliveryParticles;  // Plays when a box is successfully delivered

    void OnTriggerEnter(Collider other)
    {
        // +1
        HUDManager.IncrementScore();

        if (deliveryParticles != null)
        {
            deliveryParticles.Play();
        }
    }
}