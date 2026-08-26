using UnityEngine;
using System.IO;

using UnityEditor;

// Credits to Stack Overflow & Deepseek AI.
// I given up halfway through this after 3 hours of Youtube, forgive me

public class SaveSystem : MonoBehaviour
{
    public Transform playerTransform;
    public string saveFileName = "saveData.json";

    [Header("Reset Targets")]
    [Tooltip("Resets the elapsed session timer.")]
    public ProgressTracker progressTracker;
    [Tooltip("Resets the crane, magnet position and boxes.")]
    public CraneController craneController;
    [Tooltip("Turns the magnet visuals back to its inactive state.")]
    public MagnetVisualFeedback magnetFeedback;

    private string saveFilePath;
    private VolumeController volumeController;
    private HUDManager hudManager;

    
    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);

        volumeController = GetComponent<VolumeController>();
        hudManager = GetComponent<HUDManager>();

        if (playerTransform == null && Camera.main != null)
            playerTransform = Camera.main.transform;
    }

    public void SaveGame()
    {
        GameState state = new GameState();

        // Save player position
        if (playerTransform != null)
        {
            state.playerPosition = playerTransform.position;
            state.playerRotation = playerTransform.eulerAngles;
        }

        // Save volumes
        if (volumeController != null)
        {
            state.bgmVolume = volumeController.bgmVolume;
            state.sfxVolume = volumeController.sfxVolume;
        }

        // Save score
        if (hudManager != null)
        {
            state.score = hudManager.GetScore();
        }

        // Save elapsed session time
        if (progressTracker != null)
        {
            state.elapsedTime = progressTracker.GetElapsedTime();
        }

        // Save crane, magnet and box positions
        if (craneController != null)
        {
            state.craneState = craneController.CaptureState();
        }

        string json = JsonUtility.ToJson(state, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Game saved to {saveFilePath}");
    }

    public void LoadGame()
    {
        LoadGameData();
    }

    // Returns the training scenario to its starting state without reloading the scene.
    // Player position and audio/comfort preferences are left as the trainee has them set.
    public void ResetScene()
    {
        if (hudManager != null)
        {
            hudManager.SetScore(0);
        }

        if (progressTracker != null)
        {
            progressTracker.ResetTimer();
        }

        if (craneController != null)
        {
            craneController.ResetCrane();
        }

        if (magnetFeedback != null)
        {
            magnetFeedback.SetMagnetActiveState(false);
            magnetFeedback.OnBoxReleased();
        }

        Debug.Log("Training scenario reset to its starting state.");
    }

    public void QuitGame()
    {
        Debug.Log("Quit requested.");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LoadGameData()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("Save file not found. Nothing to load.");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        GameState state = JsonUtility.FromJson<GameState>(json);

        if (playerTransform != null)
        {
            playerTransform.position = state.playerPosition;
            playerTransform.rotation = Quaternion.Euler(state.playerRotation);
        }

        if (volumeController != null)
        {
            volumeController.bgmVolume = state.bgmVolume;
            volumeController.sfxVolume = state.sfxVolume;
            volumeController.SetBGMVolume(state.bgmVolume);
            volumeController.SetSFXVolume(state.sfxVolume);
        }

        if (hudManager != null)
        {
            hudManager.SetScore(state.score);
        }

        if (progressTracker != null)
        {
            progressTracker.SetElapsedTime(state.elapsedTime);
        }

        if (craneController != null)
        {
            craneController.ApplyState(state.craneState);
        }

        Debug.Log("Game loaded.");
    }

    public class GameState
    {
        public Vector3 playerPosition;
        public Vector3 playerRotation;
        public float bgmVolume;
        public float sfxVolume;
        public int score;
        public float elapsedTime;
        public CraneController.CraneState craneState;
    }
}