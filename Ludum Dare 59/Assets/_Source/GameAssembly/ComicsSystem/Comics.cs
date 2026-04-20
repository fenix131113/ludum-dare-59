using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ComicsSystem
{
    public class Comics : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image[] comices;
        [SerializeField] private int startComicsCount;
        [SerializeField] private bool stopTime;

        private int _currentComicsNum;
        private bool _ended;
        
        public event Action OnComicsEnded;

        private void Start()
        {
            for (var i = 0; i < startComicsCount; i++)
                comices[i].gameObject.SetActive(true);

            _currentComicsNum = startComicsCount;

            if (stopTime)
                Time.timeScale = 0f;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_currentComicsNum + 1 > comices.Length)
            {
                if(!_ended)
                {
                    gameObject.SetActive(false);
                    if (stopTime)
                        Time.timeScale = 1f;
                    OnComicsEnded?.Invoke();
                }

                _ended = true;
                return;
            }

            _currentComicsNum++;
            comices[_currentComicsNum - 1].gameObject.SetActive(true);
        }
    }
}