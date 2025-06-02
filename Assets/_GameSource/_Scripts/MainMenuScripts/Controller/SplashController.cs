using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class SplashController : MonoBehaviour {
    [Header("UI Elements")]
    public Image ProgressBar;
    public TextMeshProUGUI ProgressText;

    [Header("Settings")]
    public float SplashDuration = 2f;
    public int NextSceneIndex = 1;

    //////////////////////////////////////

    private void Start() {
        Application.targetFrameRate = 60; // Set target frame rate
        if (ProgressBar == null) {
            Debug.LogError("ProgressBar is not assigned in the inspector.");
            return;
        }
        AnimateProgressBar();
    }

    private void AnimateProgressBar() {
        // Reset progress bar
        ProgressBar.fillAmount = 0f;
        // Animate fill amount from 0 to 1
        ProgressBar.DOFillAmount(1f, SplashDuration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                if (ProgressText != null)
                    ProgressText.text = $"{Mathf.RoundToInt(ProgressBar.fillAmount * 100)}%";
            })
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    SceneManager.LoadScene(NextSceneIndex);
                });
            });
    }
}