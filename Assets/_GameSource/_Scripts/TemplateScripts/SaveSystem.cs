using UnityEngine;

public class SaveSystem : MonoBehaviour {

    private const string _bgmVolumeKey = "BGMVolume";
    private const string _sfxVolumeKey = "SFXVolume";
    private const string _levelKey = "level";
    private const string _coinKey = "coin";
    private const string _soundKey = "sound";

    // Public get-only properties
    public static string BgmVolumeKey => _bgmVolumeKey;
    public static string SfxVolumeKey => _sfxVolumeKey;
    public static string LevelKey => _levelKey;
    public static string CoinKey => _coinKey;
    public static string SoundKey => _soundKey;

    //////////////////////////////////////

    private void Awake() {
        Initialize();
    }

    public void Initialize() {
        LoadGameData();
    }

    /// Save-Load Methods

    public void SaveLevel(int level) {
        PlayerPrefs.SetInt(_levelKey, level);
        PlayerPrefs.Save();
    }

    public int LoadLevel() {
        return PlayerPrefs.GetInt(_levelKey, 1);
    }

    /// Game Currency Methods

    public void AddCoins(int amount) {
        int currentCoins = PlayerPrefs.GetInt(_coinKey, 0);
        currentCoins += amount;
        PlayerPrefs.SetInt(_coinKey, currentCoins);
        PlayerPrefs.Save();
    }

    public int GetCoins() {
        return PlayerPrefs.GetInt(_coinKey, 0);
    }

    public void LoadGameData() {
        // Load initial game data and update GameManager
        int savedLevel = LoadLevel();
        int savedCoins = GetCoins();

        if (Locator.Instance.GameManagerInstance != null) {
            // Set initial level and coins in GameManager
            Locator.Instance.GameManagerInstance.InitializeGameData(savedLevel, savedCoins);
        }
    }

    public void ResetAllData() {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}