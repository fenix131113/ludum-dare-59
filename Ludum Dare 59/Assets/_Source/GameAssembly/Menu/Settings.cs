using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menu
{
    public class Settings : MonoBehaviour
    {
        private const string MASTER_VOLUME_PREFS_KEY = "MasterVolume";
        private const string MUSIC_VOLUME_PREFS_KEY = "MusicVolume";
        private const string SOUNDS_VOLUME_PREFS_KEY = "SoundsVolume";

        private const string MASTER_VOLUME_MIXER_PARAM = "MasterVolume";
        private const string MUSIC_VOLUME_MIXER_PARAM = "MusicVolume";
        private const string SOUNDS_VOLUME_MIXER_PARAM = "SoundsVolume";

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider soundsVolumeSlider;
        [SerializeField] private TMP_Text masterVolumeLabel;
        [SerializeField] private TMP_Text musicVolumeLabel;
        [SerializeField] private TMP_Text soundsVolumeLabel;
        [SerializeField] private float minMixerValue = 0.0001f;
        [SerializeField] private float defaultVolume = 1f;

        private void Start()
        {
            LoadSettings();
            Bind();
        }

        private void OnDestroy() => Expose();

        private void OnMasterVolumeChanged(float value)
        {
            ApplyVolume(MASTER_VOLUME_MIXER_PARAM, masterVolumeLabel, value);
            SaveVolume(MASTER_VOLUME_PREFS_KEY, value);
        }

        private void OnMusicVolumeChanged(float value)
        {
            ApplyVolume(MUSIC_VOLUME_MIXER_PARAM, musicVolumeLabel, value);
            SaveVolume(MUSIC_VOLUME_PREFS_KEY, value);
        }

        private void OnSoundsVolumeChanged(float value)
        {
            ApplyVolume(SOUNDS_VOLUME_MIXER_PARAM, soundsVolumeLabel, value);
            SaveVolume(SOUNDS_VOLUME_PREFS_KEY, value);
        }

        private void LoadSettings()
        {
            var masterVolume = GetSavedOrDefaultVolume(MASTER_VOLUME_PREFS_KEY);
            var musicVolume = GetSavedOrDefaultVolume(MUSIC_VOLUME_PREFS_KEY);
            var soundsVolume = GetSavedOrDefaultVolume(SOUNDS_VOLUME_PREFS_KEY);

            if (masterVolumeSlider)
                masterVolumeSlider.value = masterVolume;
            if (musicVolumeSlider)
                musicVolumeSlider.value = musicVolume;
            if (soundsVolumeSlider)
                soundsVolumeSlider.value = soundsVolume;

            ApplyVolume(MASTER_VOLUME_MIXER_PARAM, masterVolumeLabel, masterVolume);
            ApplyVolume(MUSIC_VOLUME_MIXER_PARAM, musicVolumeLabel, musicVolume);
            ApplyVolume(SOUNDS_VOLUME_MIXER_PARAM, soundsVolumeLabel, soundsVolume);
        }

        private float GetSavedOrDefaultVolume(string prefsKey)
        {
            if (PlayerPrefs.HasKey(prefsKey))
                return Mathf.Clamp01(PlayerPrefs.GetFloat(prefsKey, defaultVolume));

            var defaultClampedVolume = Mathf.Clamp01(defaultVolume);
            SaveVolume(prefsKey, defaultClampedVolume);
            return defaultClampedVolume;
        }

        private void ApplyVolume(string mixerParam, TMP_Text volumeLabel, float value)
        {
            var clampedValue = Mathf.Clamp01(value);

            if (audioMixer)
                audioMixer.SetFloat(mixerParam, ConvertLinearToMixerValue(clampedValue));

            if (volumeLabel)
                volumeLabel.text = $"{Mathf.RoundToInt(clampedValue * 100f)}%";
        }

        private float ConvertLinearToMixerValue(float value)
        {
            var minValue = Mathf.Clamp(minMixerValue, 0.0001f, 1f);
            return Mathf.Log10(Mathf.Max(value, minValue)) * 20f;
        }

        private static void SaveVolume(string key, float value)
        {
            PlayerPrefs.SetFloat(key, Mathf.Clamp01(value));
            PlayerPrefs.Save();
        }

        private void Bind()
        {
            if (masterVolumeSlider)
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

            if (musicVolumeSlider)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

            if (soundsVolumeSlider)
                soundsVolumeSlider.onValueChanged.AddListener(OnSoundsVolumeChanged);
        }

        private void Expose()
        {
            if (masterVolumeSlider)
                masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);

            if (musicVolumeSlider)
                musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);

            if (soundsVolumeSlider)
                soundsVolumeSlider.onValueChanged.RemoveListener(OnSoundsVolumeChanged);
        }
    }
}
