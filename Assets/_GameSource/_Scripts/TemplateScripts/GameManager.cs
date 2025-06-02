using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    #region Variables

    [Header("Game State")]
    [HideInInspector] public bool IsGameStarted;

    [Header("Variables")]
    private int _currentScore;
    private int _currentLevel = 1;
    public int Level => _currentLevel;
    private int _coin;
    public int Coin => _coin;

    [Header("Actions")]
    public static Action OnGameWin;
    public static Action OnGameLose;

    #endregion

    //////////////////////////////////////

    #region Methods

    private void Awake() {
        Application.targetFrameRate = 60;
    }

    private void Start() {

    }

    public void InitializeGameData(int level, int coins) {
        _currentLevel = level;
        _coin = coins;
    }

    public void StartNewGame() {
        IsGameStarted = true;
        GenerateLevel();
    }

    /// Game State Methods

    public void FinishLevel() {
        IsGameStarted = false;
        OnGameWin?.Invoke();
        Locator.Instance.SoundManagerInstance.PlaySFX("WinSound");
        LevelUp();
    }

    public void GameOver() {
        IsGameStarted = false;
        Locator.Instance.SoundManagerInstance.PlaySFX("FailSound");
        OnGameLose?.Invoke();
    }

    /// Game Level Methods

    private void GenerateLevel() {
        int spawnIndex = _currentLevel - 1;
       // Locator.Instance.LevelManagerInstance.SpawnLevel(spawnIndex);
        Debug.Log("Level Generated: " + _currentLevel);
    }

    private void LevelUp() {
        AddCoin(50);
        _currentLevel++;
        Locator.Instance.SaveSystemInstance.SaveLevel(_currentLevel);
    }

    public void ReloadScene() => SceneManager.LoadScene(0);

    /// Game Currency Methods

    public void AddCoin(int amount) {
        _coin += amount;
        Locator.Instance.SaveSystemInstance.AddCoins(amount);
        Debug.Log("Coins Added: " + amount);
    }

    public void DiscardCoin(int amount) {
        _coin -= amount;
        Locator.Instance.SaveSystemInstance.AddCoins(-amount);
        Debug.Log("Coins Discarded: " + amount);
    }

    #endregion

}