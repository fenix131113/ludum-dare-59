using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Utils
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private bool loadOnAwake;
        [SerializeField] private int awakeSceneIndex;
        [SerializeField] private float waitingToLoadSeconds = 0.5f;

        [Inject] private SceneTransitionEffect sceneTransition;
        
        private void Awake()
        {
            if (loadOnAwake)
                SceneManager.LoadScene(awakeSceneIndex);
        }
        private void Start()
        {
            StartCoroutine(ShowScene());
        }
        public void LoadScene(int index) => StartCoroutine(LoadIntoScene(index));

        public IEnumerator LoadIntoScene(int index)
        {
            sceneTransition.HideView();
            yield return new WaitForSecondsRealtime(waitingToLoadSeconds);
            SceneManager.LoadScene(index);
        }

        public IEnumerator ShowScene()
        {
            yield return new WaitForSecondsRealtime(waitingToLoadSeconds);
            sceneTransition.ShowView();
        }
    }
}