using UnityEngine;
using VContainer;

namespace SignalSystem.View
{
    public class SignalPowerView : MonoBehaviour
    {
        [Inject] private SignalTracker _tracker;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void OnSignalPowerChanged(float oldValue, float newValue)
        {
            
        }

        private void Bind() => _tracker.OnSignalPowerChanged += OnSignalPowerChanged;

        private void Expose() => _tracker.OnSignalPowerChanged -= OnSignalPowerChanged;
    }
}