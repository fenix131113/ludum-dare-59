using UnityEngine;

namespace Utils
{
    public class TempAudioSource : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        public void PlayAndDestroy(AudioClip clip = null)
        {
            if (!clip)
            {
                audioSource.Play();
                Destroy(audioSource.gameObject, audioSource.clip.length);
                return;
            }
            
            audioSource.PlayOneShot(clip);
            Destroy(audioSource.gameObject, clip.length);
        }
    }
}