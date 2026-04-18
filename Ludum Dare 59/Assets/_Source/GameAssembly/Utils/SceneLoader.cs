using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private bool loadOnAwake;
        [SerializeField] private int awakeSceneIndex;
        
        private void Awake()
        {
            if (loadOnAwake)
                SceneManager.LoadScene(awakeSceneIndex);
        }
        
        public void LoadScene(int index) =>  SceneManager.LoadScene(index);
    }
}