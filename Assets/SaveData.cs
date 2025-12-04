using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveData : MonoBehaviour
{
    // Static state used across the game
    public static bool IsPlayNow = false;
    public static string[] WordsArray;
    public static List<string> EnteredWords = new List<string>();
    public static int CurrentLvl;
    public static int CurrentCell;

    // Serializable container for JSON
    [Serializable]
    public class PlayerData
    {
        public int MaxCombo;
        public bool IsPlayNow;
        public string[] WordsArray;
        public string[] EnteredWords;
        public int CurrentLvl;
        public int CurrentCell;
    }

    private const string SaveFileName = "saveData.json";

    private void Awake()
    {
        // Load data only in the Menu scene (Index 0) to prevent overwriting during gameplay
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            LoadGameState();
        }
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            SaveGameState();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameState();
    }

    public static void SaveGameState()
    {
        string savePath = Path.Combine(Application.persistentDataPath, SaveFileName);

        PlayerData data = new PlayerData
        {
            IsPlayNow = IsPlayNow,
            MaxCombo = PlayerStats.combo,
            WordsArray = WordsArray,
            CurrentLvl = CurrentLvl,
            // Convert list to array for serialization
            EnteredWords = EnteredWords != null ? EnteredWords.ToArray() : new string[0],
            CurrentCell = CurrentCell
        };

        try
        {
            string jsonData = JsonUtility.ToJson(data, true); // pretty print for debugging
            File.WriteAllText(savePath, jsonData);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveData] Failed to save data: {e.Message}");
        }
    }

    public static void LoadGameState()
    {
        string savePath = Path.Combine(Application.persistentDataPath, SaveFileName);

        if (File.Exists(savePath))
        {
            try
            {
                string jsonData = File.ReadAllText(savePath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(jsonData);

                // Restore state
                PlayerStats.combo = data.MaxCombo;
                CurrentLvl = data.CurrentLvl;
                WordsArray = data.WordsArray;
                IsPlayNow = data.IsPlayNow;
                EnteredWords = data.EnteredWords != null ? new List<string>(data.EnteredWords) : new List<string>();
                CurrentCell = data.CurrentCell;

                // Resume game if it was active
                if (IsPlayNow)
                {
                    SceneManager.LoadScene(1);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveData] Failed to load data (Corrupt file?): {e.Message}");
                ResetState();
            }
        }
        else
        {
            ResetState();
        }
    }

    private static void ResetState()
    {
        PlayerStats.combo = 0;
        EnteredWords = new List<string>();
        CurrentLvl = 0;
        IsPlayNow = false;
    }
}