using UnityEngine;

public class SoundManager : MonoBehaviour {
    #region Fields

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] soundEffects;

    [Header("Variables")]
    public bool IsMenu;

    #endregion

    #region Unity Methods

        private void Awake() {
            LoadVolumeSettings();
        }

        private void Start() {
            if (bgmSource == null || sfxSource == null) {
                Debug.LogError("SoundManager: AudioSources are not assigned!");
                return;
            }
            if (soundEffects.Length == 0) {
                Debug.LogWarning("SoundManager: No sound effects assigned!");
            }
            PlayBGM("Music", true);
        }

    #endregion

    #region Public Methods

        public void PlayBGM(string soundName, bool loop = true) {
            AudioClip clip = GetClipByName(soundName);
            if (clip == null) {
                return;
            }
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }

        public void StopBGM() {
            bgmSource.Stop();
        }

        public void PlaySFX(string soundName) {
            AudioClip clip = GetClipByName(soundName);
            if (clip == null) {
                return;
            }
            sfxSource.PlayOneShot(clip);
        }

        public void SetBGMVolume(float volume) {
            volume = Mathf.Clamp01(volume);
            bgmSource.volume = volume;
            PlayerPrefs.SetFloat(SaveSystem.BgmVolumeKey, volume);
        }

        public void SetSFXVolume(float volume) {
            volume = Mathf.Clamp01(volume);
            sfxSource.volume = volume;
            PlayerPrefs.SetFloat(SaveSystem.SfxVolumeKey, volume);
        }

        // Toggles Music and SFX mute states
        public void ToggleMuteMusic() {
            bool isMuted = bgmSource.volume == 0;
            float volume = isMuted ? 0.5f : 0f;
            SetBGMVolume(volume);
            MenuLocator.Instance.MenuManagerInstance.MusicButtonToggle.enabled = isMuted;
        }

        public void ToggleMuteSFX() {
            bool isMuted = sfxSource.volume == 0;
            float volume = isMuted ? 0.5f : 0f;
            SetSFXVolume(volume);
            MenuLocator.Instance.MenuManagerInstance.SFXButtonToggle.enabled = isMuted;
        }


    #endregion

    #region Private Methods

        private void LoadVolumeSettings() {
            if (PlayerPrefs.HasKey(SaveSystem.BgmVolumeKey) == false) {
                PlayerPrefs.SetFloat(SaveSystem.BgmVolumeKey, 0.5f);
            }
            if (PlayerPrefs.HasKey(SaveSystem.SfxVolumeKey) == false) {
                PlayerPrefs.SetFloat(SaveSystem.SfxVolumeKey, 0.5f);
            }
            bgmSource.volume = PlayerPrefs.GetFloat(SaveSystem.BgmVolumeKey);
            sfxSource.volume = PlayerPrefs.GetFloat(SaveSystem.SfxVolumeKey);
            bool isMutedMusic = bgmSource.volume == 0;
            bool isMutedSFX = sfxSource.volume == 0;

            // Set the toggle states in the menu if applicable
            if (IsMenu) {
                MenuLocator.Instance.MenuManagerInstance.MusicButtonToggle.enabled = !isMutedMusic;
                MenuLocator.Instance.MenuManagerInstance.SFXButtonToggle.enabled = !isMutedSFX;
            } else {
                Debug.LogWarning("SoundManager: cannot set toggle states.");
            }
           // Debug.Log($"SoundManager: Loaded BGM volume: {bgmSource.volume}, SFX volume: {sfxSource.volume}");
        }

        private AudioClip GetClipByName(string name) {
            foreach (var clip in soundEffects) {
                if (clip.name == name) {
                    return clip;
                }
            }
            Debug.LogWarning($"SoundManager: {name} audioclip not found!");
            return null;
        }

    #endregion

}