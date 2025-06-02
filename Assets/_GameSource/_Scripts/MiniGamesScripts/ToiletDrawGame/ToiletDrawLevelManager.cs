using UnityEngine;

public class ToiletDrawLevelManager : MonoBehaviour {
    [Header("Level Configuration")]
    public ToiletDrawLevelData[] LevelDatas;
    public int CurrentLevelIndex;

    private const string LEVEL_SAVE_KEY = "ToiletDrawSavedLevel";

    [Header("References")]
    private Camera _mainCamera;
    private ToiletDrawGameManager _toiletDrawGameManagerInstance;

    ////////////////////////////////////////////////

    private void Start() {
        _toiletDrawGameManagerInstance = ToiletDrawLocator.Instance.ToiletDrawGameManagerInstance;
        _mainCamera = ToiletDrawLocator.Instance.MainCameraInstance;
        LoadCurrentLevel();
    }

    public int LoadLevelNumber() {
        CurrentLevelIndex = PlayerPrefs.GetInt(LEVEL_SAVE_KEY);
        return CurrentLevelIndex;
    }

    // Setup the level based on the current index
    private void LoadCurrentLevel() {
        if (LevelDatas == null || LevelDatas.Length == 0) {
            Debug.LogError("No level data assigned to ToiletDrawLevelManager!");
            return;
        }

        LoadLevelNumber();
        Debug.Log("LEVEL " + (LoadLevelNumber() + 1));
        if (CurrentLevelIndex >= LevelDatas.Length) {
            CurrentLevelIndex = 0;
        }

        ToiletDrawLevelData currentLevelData = LevelDatas[CurrentLevelIndex];
        SetupLevel(currentLevelData);
    }

    private void SetupLevel(ToiletDrawLevelData levelData) {
        // Setup camera
        if (_mainCamera != null) {
            _mainCamera.transform.position = levelData.CameraPosition;
            _mainCamera.orthographicSize = levelData.CameraSize;
        }

        // Setup characters
        if (_toiletDrawGameManagerInstance.MaleCharacterGameObject != null)
            _toiletDrawGameManagerInstance.MaleCharacterGameObject.transform.position = levelData.MaleCharacterStartPosition;

        if (_toiletDrawGameManagerInstance.FemaleCharacterGameObject != null)
            _toiletDrawGameManagerInstance.FemaleCharacterGameObject.transform.position = levelData.FemaleCharacterStartPosition;

        // Setup toilets
        if (_toiletDrawGameManagerInstance.MaleToiletTransform != null)
            _toiletDrawGameManagerInstance.MaleToiletTransform.position = levelData.MaleToiletPosition;

        if (_toiletDrawGameManagerInstance.FemaleToiletTransform != null)
            _toiletDrawGameManagerInstance.FemaleToiletTransform.position = levelData.FemaleToiletPosition;
    }
}