using TimerSystem;
using TMPro;
using UnityEngine;
using VContainer;

namespace SignalSystem.View
{
    public class SignalsGoalView : MonoBehaviour
    {
        [SerializeField] private TMP_Text signalsLabel;

        [Inject] private GameCondition _gameCondition;

        private void Start()
        {
            Bind();
            Redraw();
        }

        private void OnDestroy() => Expose();

        private void OnCurrentSignalsChanged() => Redraw();

        private void Redraw() => signalsLabel.text = _gameCondition.GetCurrentSignalsCount().ToString() +
                                                     $"/{_gameCondition.GetNeedSignalsCount().ToString()}";

        private void Bind() => _gameCondition.OnCurrentSignalsChanged += OnCurrentSignalsChanged;

        private void Expose() => _gameCondition.OnCurrentSignalsChanged -= OnCurrentSignalsChanged;
    }
}