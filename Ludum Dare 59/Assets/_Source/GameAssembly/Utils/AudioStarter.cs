using UnityEngine;

namespace Utils
{
    public class AudioStarter : MonoBehaviour
    {
        [field: SerializeField] public TempAudioSource TempAudioSourcePrefab { get; private set; }

        private static AudioStarter _instance;

        private void Awake()
        {
            _instance = this;
        }

        public static void PlaySound(AudioClip clip, float volume = float.MinValue) =>
            Instantiate(_instance.TempAudioSourcePrefab).PlayAndDestroy(clip, volume);
    }
}