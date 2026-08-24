using UnityEngine;
using UnityEngine.UI;

public class ViewSwitcher : MonoBehaviour
{
    [Header("Navigation Buttons")]
    public Button progressButton;
    public Button settingsButton;
    public Button audioButton;

    [Header("Panels")]
    public GameObject progressPanel;
    public GameObject settingsPanel;
    public GameObject audioPanel;

    void Start()
    {
        // Default view: Progress active, Settings & Audio inactive
        ShowProgress();

        if (progressButton != null)
            progressButton.onClick.AddListener(ShowProgress);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);

        if (audioButton != null)
            audioButton.onClick.AddListener(ShowAudio);
    }

    // All the functions
    // Click 1, Inactive the rest
    public void ShowProgress()
    {
        if (progressPanel != null) progressPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (audioPanel != null) audioPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        if (progressPanel != null) progressPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (audioPanel != null) audioPanel.SetActive(false);
    }

    public void ShowAudio()
    {
        if (progressPanel != null) progressPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (audioPanel != null) audioPanel.SetActive(true);
    }
}