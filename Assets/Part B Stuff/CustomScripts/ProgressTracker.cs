using UnityEngine;
using TMPro;

public class ProgressTracker : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Drag your TextMeshPro Text UI here to display the time.")]
    public TextMeshProUGUI timeText;

    private float elapsedTime = 0f;
    private bool isTracking = true;

    private void Update()
    {
        if (isTracking)
        {
            // Increase the time
            elapsedTime += Time.deltaTime;

            // Update the UI if we have a text component attached
            if (timeText != null)
            {
                // Format time as Minutes:Seconds (e.g., 01:23)
                int minutes = Mathf.FloorToInt(elapsedTime / 60F);
                int seconds = Mathf.FloorToInt(elapsedTime - minutes * 60);
                string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);
                
                timeText.text = "Time: " + niceTime;
            }
        }
    }

    // Optional: Call this if you ever want to stop the timer
    public void StopTimer()
    {
        isTracking = false;
    }

    // For saving/loading the timer's progress
    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void SetElapsedTime(float time)
    {
        elapsedTime = time;
        isTracking = true;

        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60F);
            int seconds = Mathf.FloorToInt(elapsedTime - minutes * 60);
            timeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    // Call this to return the timer to its starting state without reloading the scene.
    public void ResetTimer()
    {
        elapsedTime = 0f;
        isTracking = true;

        if (timeText != null)
        {
            timeText.text = "Time: 00:00";
        }
    }
}
