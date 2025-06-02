using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    [Header("UI Panels")]
    [SerializeField] private Canvas menuPanelCanvas;
    [SerializeField] private GraphicRaycaster menuPanelGraphicRaycaster;
    [SerializeField] private Canvas settingsPanelCanvas;
    [SerializeField] private GraphicRaycaster settingsGraphicRaycaster;

    [Header("UI Objects")]
    public Image MusicButtonToggle;
    public Image SFXButtonToggle;

    //////////////////////////////////////

    private void Start() {

    }

    public void OnPressedSettingsOpenButton() {
        settingsPanelCanvas.enabled = true;
        settingsGraphicRaycaster.enabled = true;
    }

    public void OnPressedSettingsCloseButton() {
        settingsPanelCanvas.enabled = false;
        settingsGraphicRaycaster.enabled = false;
    }

    // Music and SFX Buttons
    public void OnPressedMusicButton() {
        MenuLocator.Instance.SoundManagerInstance.ToggleMuteMusic();
    }

    public void OnPressedSFXButton() {
        MenuLocator.Instance.SoundManagerInstance.ToggleMuteSFX();
    }

    // Placeholder
    public void OnPressedRemoveAdsButton() {
        // Implement the logic to remove ads
        Debug.Log("Remove Ads button pressed.");
    }

    // Placeholder
    public void OnPressedRestorePurchaseButton() {
        // Implement the logic to restore purchases
        Debug.Log("Restore Purchase button pressed.");
    }

    // Placeholder
    public void OnPressedPrivacyPolicyButton() {
        Application.OpenURL("https://example.com/privacy-policy");
    }

}