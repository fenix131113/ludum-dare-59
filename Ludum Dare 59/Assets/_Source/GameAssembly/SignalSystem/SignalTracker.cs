using System;
using MiniGames;
using MiniGames.Games.RepeatPictures;
using Player;
using Player.Data;
using UnityEngine;
using Utils.VariablesSystem;
using VContainer;

namespace SignalSystem
{
    public class SignalTracker : MonoBehaviour
    {
        [SerializeField] private float maxTrackDistance;
        [SerializeField] private Transform trackFromTarget;

        [Inject] private SignalHolder _signalHolder;
        [Inject] private PlayerInput _playerInput;
        [Inject] private IVariablesResolver<PlayerVariableBlockerType, Action, Action> _playerVariables;

        private float _signalPower;

        public event Action<float, float> OnSignalPowerChanged;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void Update()
        {
            if (!_signalHolder.GetCurrentSignal())
                return;

            var signal = _signalHolder.GetCurrentSignal();

            var minTrackDistance = signal.GetSignalRadius();
            var distance = Vector2.Distance(trackFromTarget.position, signal.transform.position);
            var from = distance > minTrackDistance ? distance : minTrackDistance;
            var result = 1f - Mathf.Clamp((from - minTrackDistance) / (maxTrackDistance - minTrackDistance), 0, 1);

            if (!Mathf.Approximately(result, _signalPower))
                SetSignalPower(result);
        }

        private void OnSendSignalClicked()
        {
            if (!Mathf.Approximately(_signalPower, 1f) || !_signalHolder.GetCurrentSignal() ||
                _playerVariables.IsVariableBlocked(PlayerVariableBlockerType.SEND_SIGNAL))
                return;

            FindFirstObjectByType<MinigamesManager>().PlayMinigame(FindFirstObjectByType<RepeatSymbolsMinigame>(FindObjectsInactive.Include), _signalHolder.GetCurrentSignal());
        }

        private void SetSignalPower(float power)
        {
            var temp = _signalPower;
            _signalPower = power;
            OnSignalPowerChanged?.Invoke(temp, power);
        }

        public float GetSignalPower01() => _signalPower;

        private void Bind() => _playerInput.OnSendSignalClicked += OnSendSignalClicked;

        private void Expose() => _playerInput.OnSendSignalClicked -= OnSendSignalClicked;
    }
}