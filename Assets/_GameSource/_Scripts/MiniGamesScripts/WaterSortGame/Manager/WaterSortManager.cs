using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class WaterSortManager : MonoBehaviour {

    public int LevelNumber { get; private set; }

    [Header("VARIABLES")]
    public bool IsGamePause;
    private bool _isLevelCompleted;

    [Header("OBJECTS")]
    [SerializeField] private Canvas nextLevelPanelCanvas;
    [SerializeField] private GraphicRaycaster nextLevelRaycaster;

    private void Awake() {
        QualitySettings.vSyncCount = 0;
        LoadData();
    }

    private void Start() {

    }

    public void OnPressedNextLevelButton() {
        IsGamePause = false;
        SaveData();
        SceneManager.LoadScene(4);
    }

    public void OnPressedHomeButton() {
        SaveData();
        SceneManager.LoadScene(1);
    }

    public void OnPressedRestartButton() {
        SaveData();
        SceneManager.LoadScene(4);
    }

    public void SaveData() {
        PlayerPrefs.SetInt("level", LevelNumber);
        PlayerPrefs.Save();
    }

    private void LoadData() {
        LevelNumber = PlayerPrefs.GetInt("level", 0);
        //Debug.Log($"Loaded level index: {LevelNumber}");
    }

    public void NextLevel() {
        if (!_isLevelCompleted) {
            _isLevelCompleted = true;
            LevelNumber++;
            SaveData();
        }
        WaterSortLocator.Instance.SoundManagerInstance.PlaySFX("WinSound");
        nextLevelPanelCanvas.enabled = true;
        nextLevelRaycaster.enabled = true;
    }
}