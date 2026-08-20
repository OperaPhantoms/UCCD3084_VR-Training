using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles the visual feedback for the Magnet (Part A Item 6).
/// Changes color when active/inactive, and plays particles when actively lifting a box.
/// </summary>
public class MagnetVisualFeedback : MonoBehaviour
{
    [Header("Visual State (Color Change)")]
    [Tooltip("The Renderer of the magnet body whose material will change")]
    public MeshRenderer magnetRenderer;
    [Tooltip("Material when the magnet is ON")]
    public Material activeMaterial;
    [Tooltip("Material when the magnet is OFF")]
    public Material inactiveMaterial;

    [Header("Particle Effect (Sparks/Field)")]
    [Tooltip("The Particle System that plays when a box is lifted")]
    public ParticleSystem liftingParticles;

    /// <summary>
    /// Call this from the "Magnet On/Off" button or toggle event.
    /// Check the 'Dynamic bool' option in the Unity Event if it provides one,
    /// or explicitly call this with 'true' for ON and 'false' for OFF.
    /// </summary>
    public void SetMagnetActiveState(bool isActive)
    {
        if (magnetRenderer != null)
        {
            magnetRenderer.material = isActive ? activeMaterial : inactiveMaterial;
        }
        else
        {
            Debug.LogWarning("MagnetVisualFeedback: Magnet Renderer is not assigned!");
        }
    }

    /// <summary>
    /// Call this from the XRSocketInteractor's 'Select Entered' event.
    /// This triggers when a box attaches to the magnet.
    /// </summary>
    public void OnBoxGrabbed(SelectEnterEventArgs args)
    {
        if (liftingParticles != null)
        {
            liftingParticles.Play();
        }
        else
        {
            Debug.LogWarning("MagnetVisualFeedback: Lifting Particles system is not assigned!");
        }
    }

    /// <summary>
    /// Call this from the XRSocketInteractor's 'Select Exited' event.
    /// This triggers when a box detaches from the magnet.
    /// </summary>
    public void OnBoxReleased(SelectExitEventArgs args)
    {
        if (liftingParticles != null)
        {
            liftingParticles.Stop();
        }
    }

    // Overloaded methods in case the event doesn't pass SelectEnterEventArgs
    public void OnBoxGrabbed()
    {
        if (liftingParticles != null) liftingParticles.Play();
    }

    public void OnBoxReleased()
    {
        if (liftingParticles != null) liftingParticles.Stop();
    }
}
