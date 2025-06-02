using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ToiletDrawUIManager : MonoBehaviour {

    [Header("UI Panels")]
    [SerializeField] private Canvas gamePanelCanvas;
    [SerializeField] private GraphicRaycaster gamePanelGraphicRaycaster;
    [SerializeField] private Canvas winPanelCanvas;
    [SerializeField] private GraphicRaycaster winPanelGraphicRaycaster;
    [SerializeField] private Canvas gameOverPanelCanvas;
    [SerializeField] private GraphicRaycaster gameOverPanelGraphicRaycaster;

    [Header("Texts")]
    public TextMeshProUGUI LevelText;

    ////////////////////////////////////////////////

    #region Unity Methods

    private void OnEnable() {
        ToiletDrawGameManager.OnGameWin += HandleGameWin;
        ToiletDrawGameManager.OnGameLose += HandleGameLose;
    }

    private void OnDisable() {
        ToiletDrawGameManager.OnGameWin -= HandleGameWin;
        ToiletDrawGameManager.OnGameLose -= HandleGameLose;
    }

    private void Start() {
        UpdateLevelUI();
    }

    #endregion

    #region Event Handlers

    private void HandleGameWin() {
        gamePanelCanvas.enabled = false;
        gamePanelGraphicRaycaster.enabled = false;
        winPanelCanvas.enabled = true;
        winPanelGraphicRaycaster.enabled = true;
    }

    private void HandleGameLose() {
        gamePanelCanvas.enabled = false;
        gamePanelGraphicRaycaster.enabled = false;
        gameOverPanelCanvas.enabled = true;
        gameOverPanelGraphicRaycaster.enabled = true;
    }

    #endregion

    #region UI Button Methods

    public void OnPressedNextLevelButton() {
        SceneManager.LoadScene(3);
    }

    public void OnPressedHomeButton() {
        SceneManager.LoadScene(1);
    }

    public void OnPressedRestartButton() {
        SceneManager.LoadScene(3);
    }

    private void UpdateLevelUI() => LevelText.text = "LEVEL " + (ToiletDrawLocator.Instance.ToiletDrawLevelManagerInstance.LoadLevelNumber() + 1);

    #endregion
}