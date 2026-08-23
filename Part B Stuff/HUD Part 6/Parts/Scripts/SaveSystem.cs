using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

using UnityEditor;

// Credits to Stack Overflow & Deepseek AI.
// I given up halfway through this after 3 hours of Youtube, forgive me

public class SaveSystem : MonoBehaviour
{
    public Transform playerTransform;
    public string saveFileName = "saveData.json";
    private string saveFilePath;
    private static bool loadOnStart = false;
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

    private void Start()
    {
        if (loadOnStart)
        {
            LoadGameData();
            loadOnStart = false;
        }
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

        string json = JsonUtility.ToJson(state, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Game saved to {saveFilePath}");
    }

    public void LoadGame()
    {
        loadOnStart = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

        Debug.Log("Game loaded.");
    }

    public class GameState
    {
        public Vector3 playerPosition;
        public Vector3 playerRotation;
        public float bgmVolume;
        public float sfxVolume;
        public int score;
    }
}