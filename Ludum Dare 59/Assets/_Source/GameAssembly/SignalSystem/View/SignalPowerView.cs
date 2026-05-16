using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace SignalSystem.View
{
    public class SignalPowerView : MonoBehaviour
    {
        [SerializeField] private Image signalImage;
        [SerializeField] private Sprite[] progressSprites;
        
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
            if (Mathf.Approximately(power, 1f))
            {
                signalImage.sprite = progressSprites[^1];
                return;
            }

            var index = power switch
            {
                >= 0.2f and < 0.4f => 1,
                >= 0.4f and < 0.6f => 2,
                >= 0.6f and < 0.8f => 3,
                >= 0.8f and < 1f => 4,
                _ => 0
            };

            signalImage.sprite = progressSprites[index];
        }

        private void Bind() => _tracker.OnSignalPowerChanged += OnSignalPowerChanged;

        private void Expose() => _tracker.OnSignalPowerChanged -= OnSignalPowerChanged;
    }
}