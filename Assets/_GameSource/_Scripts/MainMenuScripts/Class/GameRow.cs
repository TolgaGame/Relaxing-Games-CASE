using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameRow : MonoBehaviour {

    [Tooltip("Maximum number of mini-games that can be displayed in a row 2 pieces")]
    [Header("MINI GAME DATA")]
    public MiniGameData MiniGameData1;
    public MiniGameData MiniGameData2;

    [Header("MINI GAME SLOT 1")]
    public TextMeshProUGUI MiniGameTitleText;
    public Image MiniGameImage;
    public Button MiniGameButton;

    [Space]
    [Header("MINI GAME SLOT 2")]
    public TextMeshProUGUI MiniGameTitleText2;
    public Image MiniGameImage2;
    public Button MiniGameButton2;

    //////////////////////////////////////

    private void Start() {
        MiniGameTitleText.text = MiniGameData1.MiniGameName;
        MiniGameTitleText2.text = MiniGameData2.MiniGameName;
        MiniGameImage.sprite = MiniGameData1.MiniGameSprite;
        MiniGameImage2.sprite = MiniGameData2.MiniGameSprite;
        MiniGameButton.onClick.AddListener(() => OnPressedMiniGameButton(MiniGameData1.MiniGameIndex));
        MiniGameButton2.onClick.AddListener(() => OnPressedMiniGameButton(MiniGameData2.MiniGameIndex));
    }

    // Load the selected mini-game scene
    public void OnPressedMiniGameButton(int miniGameIndex) {
        if (miniGameIndex == 999) // Empty Game
            return;
        // Assuming the first two scenes are not mini-games
        SceneManager.LoadScene(miniGameIndex + 2);
    }


}