using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace SignalSystem.View
{
    public class SignalPowerView : MonoBehaviour
    {
        [SerializeField] private Image[] signalParts;
        
        [Inject] private SignalTracker _tracker;

        private void Start()
        {
            Bind();
            RedrawSignal(0);
        }

        private void OnDestroy() => Expose();

        private void OnSignalPowerChanged(float _, float newValue)
        {
            RedrawSignal(newValue);
        }

        private void RedrawSignal(float power)
        {
            foreach (var signalPart in signalParts)
                signalPart.gameObject.SetActive(false);

            for (var i = 0; i < power / 0.2f; i++)
                signalParts[i].gameObject.SetActive(true);
        }

        private void Bind() => _tracker.OnSignalPowerChanged += OnSignalPowerChanged;

        private void Expose() => _tracker.OnSignalPowerChanged -= OnSignalPowerChanged;
    }
}