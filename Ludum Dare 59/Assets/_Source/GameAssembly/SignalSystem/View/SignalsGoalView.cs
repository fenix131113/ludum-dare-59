using DG.Tweening;
using TimerSystem;
using TMPro;
using UnityEngine;
using VContainer;

namespace SignalSystem.View
{
    public class SignalsGoalView : MonoBehaviour
    {
        [SerializeField] private TMP_Text signalsLabel;
        [SerializeField] private float popTime;
        [SerializeField] private float popSize;
        [SerializeField] private Ease popEase;

        [Inject] private GameCondition _gameCondition;

        private Tween _popTween;

        private void Start()
        {
            Bind();
            Redraw();
        }

        private void Animate()
        {
            _popTween?.Kill(true);
            _popTween = signalsLabel.transform.DOPunchScale(Vector3.one * popSize, popTime)
                .SetEase(popEase);
        }

        private void OnDestroy() => Expose();

        private void OnCurrentSignalsChanged()
        {
            Animate();
            Redraw();
        }

        private void Redraw() => signalsLabel.text = _gameCondition.GetCurrentSignalsCount().ToString() +
                                                     $"/{_gameCondition.GetNeedSignalsCount().ToString()}";

        private void Bind() => _gameCondition.OnCurrentSignalsChanged += OnCurrentSignalsChanged;

        private void Expose() => _gameCondition.OnCurrentSignalsChanged -= OnCurrentSignalsChanged;
    }
}