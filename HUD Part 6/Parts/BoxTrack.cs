using UnityEngine;
using TMPro;
using System.IO;

public class BoxCounterManager : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI countText;

    [Header("Counter Settings")]
    public int currentCount = 0;

    [Header("Save Settings")]
    public string saveFileName = "boxcount.json";   // hardcoded for now

    private string SaveFilePath
    {
        get { return Path.Combine(Application.persistentDataPath, saveFileName); }
    }

    void Start()
    {
        UpdateCountDisplay();
    }

    public void IncrementCount()
    {
        currentCount++;
        UpdateCountDisplay();
        Debug.Log($"Box count: {currentCount}");
    }

    public void SaveCount()
    {
        // Create a simple data holder
        CountData data = new CountData();
        data.count = currentCount;

        // Convert to JSON and write to file
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(SaveFilePath, json);

        Debug.Log($"Saved count {currentCount} to {SaveFilePath}");
    }

    public void LoadCount()
    {
        if (File.Exists(SaveFilePath))
        {
            string json = File.ReadAllText(SaveFilePath);
            CountData data = JsonUtility.FromJson<CountData>(json);
            currentCount = data.count;
            UpdateCountDisplay();
            Debug.Log($"Loaded count {currentCount} from {SaveFilePath}");
        }
        else
        {
            Debug.LogWarning("No save file found. Loading default 0.");
            currentCount = 0;
            UpdateCountDisplay();
        }
    }

    public void ResetCount()
    {
        currentCount = 0;
        UpdateCountDisplay();
        Debug.Log("Count reset to 0");
    }

    private void UpdateCountDisplay()
    {
        if (countText != null)
            countText.text = currentCount.ToString();
    }

    // Simple serializable class for JSON
    [System.Serializable]
    private class CountData
    {
        public int count;
    }
}