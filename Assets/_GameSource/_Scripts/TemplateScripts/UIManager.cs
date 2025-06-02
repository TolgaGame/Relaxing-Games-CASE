using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {

    [Header("UI Panels")]
    [SerializeField] private Canvas menuPanelCanvas;
    [SerializeField] private GraphicRaycaster menuPanelGraphicRaycaster;
    [SerializeField] private Canvas gamePanelCanvas;
    [SerializeField] private GraphicRaycaster gamePanelGraphicRaycaster;
    [SerializeField] private Canvas winPanelCanvas;
    [SerializeField] private GraphicRaycaster winPanelGraphicRaycaster;
    [SerializeField] private Canvas gameOverPanelCanvas;
    [SerializeField] private GraphicRaycaster gameOverPanelGraphicRaycaster;

    [Header("Texts")]
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI CoinText;

    ////////////////////////////////////////////////

    #region Unity Methods

    private void OnEnable() {
        GameManager.OnGameWin += HandleGameWin;
        GameManager.OnGameLose += HandleGameLose;
    }

    private void OnDisable() {
        GameManager.OnGameWin -= HandleGameWin;
        GameManager.OnGameLose -= HandleGameLose;
    }

    private void Start() {
        UpdateCoinUI();
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

    public void OnPressedStartButton() {
        menuPanelCanvas.enabled = false;
        menuPanelGraphicRaycaster.enabled = false;
        gamePanelCanvas.enabled = true;
        gamePanelGraphicRaycaster.enabled = true;
        // Start the game through GameManager which will handle level generation
        Locator.Instance.GameManagerInstance.StartNewGame();
        UpdateLevelUI();
        Debug.Log("Game Started");
        Debug.Log("Current Level: " + Locator.Instance.GameManagerInstance.Level);
    }

    public void OnPressedNextLevelButton() {
        Locator.Instance.GameManagerInstance.ReloadScene();
    }

    public void OnPressedContinueButton() {
        Locator.Instance.GameManagerInstance.IsGameStarted = true;
        Locator.Instance.GameManagerInstance.DiscardCoin(25);
        UpdateCoinUI();
    }

    public void OnPressedRestartButton() {
        Locator.Instance.GameManagerInstance.ReloadScene();
    }

    private void UpdateCoinUI() => CoinText.text = Locator.Instance.GameManagerInstance.Coin.ToString();

    private void UpdateLevelUI() => LevelText.text = "LEVEL " + Locator.Instance.GameManagerInstance.Level;


    #endregion

}