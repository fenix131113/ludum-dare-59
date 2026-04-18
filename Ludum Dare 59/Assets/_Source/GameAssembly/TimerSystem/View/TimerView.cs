using System;
using System.Collections;
using TMPro;
using UnityEngine;
using VContainer;

namespace TimerSystem.View
{
    public class TimerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerLabel;
        [SerializeField] private bool isGameTimer;
        [SerializeField] private float drawInterval;

        [Inject] private GameTimerCondition _gameTimerCondition;

        private bool _subscribed;

        private void Start()
        {
            DrawTimer();
            StartCoroutine(RedrawCoroutine());
        }
        
        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private void DrawTimer()
        {
            var timeLeft = isGameTimer
                ? _gameTimerCondition.GetGameTimer() != null ? _gameTimerCondition.GetGameTimer().TimeLeft : 0f
                : _gameTimerCondition.GetSignalTimer() != null
                    ? _gameTimerCondition.GetSignalTimer().TimeLeft
                    : 0f;
            
            timerLabel.text = Mathf.CeilToInt(timeLeft).ToString();
        }

        private IEnumerator RedrawCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(drawInterval);
            
                DrawTimer();   
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}