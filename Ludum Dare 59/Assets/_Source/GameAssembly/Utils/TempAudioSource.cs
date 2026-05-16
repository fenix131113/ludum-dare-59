using UnityEngine;

namespace Utils
{
    public class TempAudioSource : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        public void PlayAndDestroy(AudioClip clip = null,  float volume = float.MinValue)
        {
            if (!clip)
            {
                audioSource.Play();
                Destroy(audioSource.gameObject, audioSource.clip.length);
                return;
            }
            
            if(Mathf.Approximately(float.MinValue, volume))
                audioSource.PlayOneShot(clip);
            else
                audioSource.PlayOneShot(clip, volume);
            
            Destroy(audioSource.gameObject, clip.length);
        }
    }
}