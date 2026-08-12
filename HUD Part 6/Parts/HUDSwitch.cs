using UnityEngine;
using UnityEngine.UI;

public class ViewSwitcher : MonoBehaviour
{
    [Header("Navigation Buttons")]
    public Button progressButton;
    public Button settingsButton;

    [Header("Panels")]
    public GameObject progressPanel;
    public GameObject settingsPanel;

    void Start()
    {
        // Default view: Progress active, Settings inactive
        ShowProgress();

        if (progressButton != null)
            progressButton.onClick.AddListener(ShowProgress);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);
    }

    public void ShowProgress()
    {
        if (progressPanel != null) progressPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        if (progressPanel != null) progressPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }
}