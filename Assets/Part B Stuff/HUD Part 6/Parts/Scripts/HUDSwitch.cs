using UnityEngine;
using UnityEngine.UI;

public class ViewSwitcher : MonoBehaviour
{
    public Button progressButton;
    public Button settingsButton;
    public Button audioButton;

    public GameObject progressPanel;
    public GameObject settingsPanel;
    public GameObject audioPanel;

    void Start()
    {
        //welcome back to datacomm
        ShowProgress(); //default
        progressButton.onClick.AddListener(ShowProgress);
        settingsButton.onClick.AddListener(ShowSettings);
        audioButton.onClick.AddListener(ShowAudio);
    }

    // Click 1, Inactive the rest
    public void ShowProgress()
    {
        progressPanel.SetActive(true);
        settingsPanel.SetActive(false);
        audioPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        progressPanel.SetActive(false);
        settingsPanel.SetActive(true);
        audioPanel.SetActive(false);
    }

    public void ShowAudio()
    {
        progressPanel.SetActive(false);
        settingsPanel.SetActive(false);
        audioPanel.SetActive(true);
    }
}