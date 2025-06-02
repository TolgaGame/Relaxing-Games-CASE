using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GunSoundGameManager : MonoBehaviour {

    [Header("OBJECTS")]
    public Image MainGunImage;
    public AudioSource GunAudioSource;
    public ParticleSystem GunParticle;
    public Transform GunFireFXTransform;
    public Transform GunBulletTransform;

    [Header("GUN DATA AND OBJECTS")]
    public GameObject WeaponContent;
    public GameObject NewWeaponButtonPrefab;
    public GunData[] GunDataList;

    // Original positions for animations
    private Vector3 originalBulletPosition;
    private Vector3 originalFireFXPosition;

    //////////////////////////////////////

    private void Start() {
        if (GunBulletTransform != null)
            originalBulletPosition = GunBulletTransform.localPosition;
        if (GunFireFXTransform != null)
            originalFireFXPosition = GunFireFXTransform.localPosition;

        // Initialize the gun data list
        if (GunDataList == null || GunDataList.Length == 0) {
            Debug.LogError("GunDataList is not set or empty.");
            return;
        }
        InitializeWeaponContent();

        // Default weapon set (index 0)
        if (MainGunImage != null && GunDataList.Length > 0) {
            MainGunImage.sprite = GunDataList[0].GunSprite;
        }
        MainGunImage.sprite = GunDataList[0].GunSprite;
        GunAudioSource.clip = GunDataList[0].GunSound;
        LoadVolumeSettings();
    }

    // Initialize the weapon content with buttons for each gun
    private void InitializeWeaponContent() {
        GameObject NewWeaponButton = NewWeaponButtonPrefab;
        if (NewWeaponButton == null) {
            Debug.LogError("Could not find NewGun prefab in Resources folder");
            return;
        }
        // Create buttons for each gun
        for (int i = 0; i < GunDataList.Length; i++) {
            GameObject newWeaponButton = Instantiate(NewWeaponButton, WeaponContent.transform);

            Image buttonImage = newWeaponButton.GetComponent<Image>();
            if (buttonImage != null) {
                buttonImage.sprite = GunDataList[i].GunSprite;
            }

            Button button = newWeaponButton.GetComponent<Button>();
            if (button != null) {
                int index = i;
                button.onClick.AddListener(() => OnPressedSelectWeaponButton(index));
            }

            newWeaponButton.SetActive(true);

            if (!GunDataList[i].IsUnlocked) {
                newWeaponButton.GetComponent<Button>().interactable = false;
            }
        }
    }

    // Trigger main gun image
    public void OnPressedMainGunImage() {
        GunAudioSource.Play();
        if (GunParticle != null) {
            GunParticle.Play();
        }

        #region Recoil and Fire Animation

        if (MainGunImage != null) {
            // Save original position, scale, and rotation for the recoil animation
            Vector3 originalPosition = MainGunImage.transform.localPosition;
            Vector3 originalScale = MainGunImage.transform.localScale;
            Vector3 currentRotation = MainGunImage.transform.localEulerAngles;

            Sequence recoilSequence = DOTween.Sequence();

            recoilSequence.Append(MainGunImage.transform.DOLocalMoveX(originalPosition.x - 15f, 0.1f))
                .Join(MainGunImage.transform.DOScale(originalScale * 0.95f, 0.1f))
                .Join(MainGunImage.transform.DOLocalRotate(new Vector3(0, 0, currentRotation.z + 5f), 0.1f))
                .Append(MainGunImage.transform.DOLocalMoveX(originalPosition.x, 0.2f))
                .Join(MainGunImage.transform.DOScale(originalScale, 0.2f))
                .Join(MainGunImage.transform.DOLocalRotate(new Vector3(0, 0, currentRotation.z), 0.2f))
                .SetEase(Ease.OutQuad);
        }

        if (GunFireFXTransform != null) {
            GunFireFXTransform.gameObject.SetActive(true);
            DOVirtual.DelayedCall(0.2f, () => GunFireFXTransform.gameObject.SetActive(false));
        }

        if (GunBulletTransform != null) {
            GunBulletTransform.gameObject.SetActive(true);

            GunBulletTransform.DOLocalRotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad);

            GunBulletTransform.DOLocalMove(
                originalBulletPosition + new Vector3(750f, 10f, 0f),
                0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    // Reset position and rotation after animation
                    GunBulletTransform.localPosition = originalBulletPosition;
                    GunBulletTransform.localRotation = Quaternion.identity;
                    GunBulletTransform.gameObject.SetActive(false);
                });
        }

        #endregion
    }

    // This will load the main menu scene
    public void OnPressedHomeButton() {
        SceneManager.LoadScene(1);
    }

    // Select new weapon
    public void OnPressedSelectWeaponButton(int weaponIndex) {
        if (weaponIndex < 0 || weaponIndex >= GunDataList.Length)
            return;

        MainGunImage.sprite = GunDataList[weaponIndex].GunSprite;
        GunAudioSource.clip = GunDataList[weaponIndex].GunSound;
    }

    private void LoadVolumeSettings() {
        GunAudioSource.volume = PlayerPrefs.GetFloat(SaveSystem.SfxVolumeKey, 0.5f);
    }
}