using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// When the scene is played, run some specific functionality
/// </summary>
public class OnSceneLoad : MonoBehaviour
{
    // When scene is loaded and play begins
    public UnityEvent OnLoad = new UnityEvent();

    // SceneManager.sceneLoaded is not raised for the first scene at application startup,
    // only for scenes loaded afterwards via LoadScene, so fire directly from Start instead.
    private void Start()
    {
        OnLoad.Invoke();
    }
}
