using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveSystem : MonoBehaviour
{
    [Header("Player")]
    public Transform playerTransform;

    [Header("Save File")]
    public string saveFileName = "saveData.json";

    private string saveFilePath;
    private static bool loadOnStart = false;

    private VolumeController volumeController;
    private HUDManager hudManager;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);

        // Get references to the other scripts on the same HUD object
        volumeController = GetComponent<VolumeController>();
        hudManager = GetComponent<HUDManager>();

        // If player not assigned, try to use main camera
        if (playerTransform == null && Camera.main != null)
            playerTransform = Camera.main.transform;
    }

    private void Start()
    {
        // If Load was requested before scene reload, apply the saved data now
        if (loadOnStart)
        {
            LoadGameData();
            loadOnStart = false;
        }
    }

    /// <summary>
    /// Saves player position, volumes, and score to a JSON file.
    /// </summary>
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

        string json = JsonUtility.ToJson(state, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Game saved to {saveFilePath}");
    }

    /// <summary>
    /// Sets the flag and reloads the scene. After reload, saved data will be applied.
    /// </summary>
    public void LoadGame()
    {
        loadOnStart = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Reloads the scene without applying saved data (full reset).
    /// </summary>
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Quits the application or stops play mode in editor.
    /// </summary>
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

        // Restore player position
        if (playerTransform != null)
        {
            playerTransform.position = state.playerPosition;
            playerTransform.rotation = Quaternion.Euler(state.playerRotation);
        }

        // Restore volumes
        if (volumeController != null)
        {
            volumeController.bgmVolume = state.bgmVolume;
            volumeController.sfxVolume = state.sfxVolume;
            // Update actual AudioSource volumes
            volumeController.SetBGMVolume(state.bgmVolume);
            volumeController.SetSFXVolume(state.sfxVolume);
        }

        // Restore score
        if (hudManager != null)
        {
            hudManager.SetScore(state.score);
        }

        Debug.Log("Game loaded.");
    }

    [System.Serializable]
    public class GameState
    {
        public Vector3 playerPosition;
        public Vector3 playerRotation;
        public float bgmVolume;
        public float sfxVolume;
        public int score;
    }
}