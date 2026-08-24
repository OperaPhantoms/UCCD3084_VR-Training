using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ManualVignetteToggle : MonoBehaviour, ITunnelingVignetteProvider
{
    [SerializeField] private TunnelingVignetteController controller;
    [SerializeField] private VignetteParameters m_VignetteParameters = new VignetteParameters();

    public VignetteParameters vignetteParameters => m_VignetteParameters;

    // Called from Toggle's OnValueChanged with dynamic bool
    public void SetVignette(bool enable)
    {
        if (enable)
            EnableVignette();
        else
            DisableVignette();
    }

    public void EnableVignette()
    {
        if (controller != null)
            controller.BeginTunnelingVignette(this);
    }

    public void DisableVignette()
    {
        if (controller != null)
            controller.EndTunnelingVignette(this);
    }
}