using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonFunctions : MonoBehaviour
{
    [Header("Manager Reference")]
    public BoxCounterManager boxCounterManager;

    [Header("Settings Buttons")]
    public Button saveButton;
    public Button loadButton;
    public Button resetButton;
    public Button quitButton;
    public Button jumpscareButton;

    [Header("Audio")]
    public AudioSource audioSource;          // assign an AudioSource on CompleteHUD
    public AudioClip jumpscareClip;          // assign the scary sound

    void Start()
    {
        // Assign button listeners
        if (saveButton != null) saveButton.onClick.AddListener(OnSaveClicked);
        if (loadButton != null) loadButton.onClick.AddListener(OnLoadClicked);
        if (resetButton != null) resetButton.onClick.AddListener(OnResetClicked);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
        if (jumpscareButton != null) jumpscareButton.onClick.AddListener(OnJumpscareClicked);
    }

    private void OnSaveClicked()
    {
        if (boxCounterManager != null)
            boxCounterManager.SaveCount();
    }

    private void OnLoadClicked()
    {
        if (boxCounterManager != null)
            boxCounterManager.LoadCount();
    }

    private void OnResetClicked()
    {
        if (boxCounterManager != null)
            boxCounterManager.ResetCount();
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit requested");
        // In the editor, stop play mode. In a build, quit the application.
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void OnJumpscareClicked()
    {
        if (audioSource != null && jumpscareClip != null)
        {
            audioSource.PlayOneShot(jumpscareClip);
            Debug.Log("Jumpscare audio played");
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip not assigned for jumpscare!");
        }
    }
}