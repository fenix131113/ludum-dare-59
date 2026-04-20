using System.Collections;
using DG.Tweening;
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
        [SerializeField] private float popTime;
        [SerializeField] private float popSize;
        [SerializeField] private Ease popEase;

        [Inject] private GameCondition _gameCondition;

        private bool _subscribed;
        private int _lastTime = -1;
        private Tween _popTween;

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
                ? _gameCondition.GetGameTimer() != null ? _gameCondition.GetGameTimer().TimeLeft : 0f
                : _gameCondition.GetSignalTimer() != null
                    ? _gameCondition.GetSignalTimer().TimeLeft
                    : 0f;

            var temp = _lastTime;
            _lastTime = Mathf.CeilToInt(timeLeft);
            timerLabel.text = _lastTime.ToString();
            
            if(temp != _lastTime && _lastTime != -1)
                Animate();
        }
        
        private void Animate()
        {
            _popTween?.Kill(true);
            _popTween = timerLabel.transform.DOPunchScale(Vector3.one * popSize, popTime)
                .SetEase(popEase);
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