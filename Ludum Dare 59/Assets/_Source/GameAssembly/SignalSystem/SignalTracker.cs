using System;
using System.Linq;
using MiniGames;
using MiniGames.Games.DoorPassword;
using Player;
using Player.Data;
using UnityEngine;
using Utils.Extensions;
using Utils.VariablesSystem;
using VContainer;

namespace SignalSystem
{
    public class SignalTracker : MonoBehaviour
    {
        [SerializeField] private float maxTrackDistance;
        [SerializeField] private Transform trackFromTarget;
        [SerializeField] private GameObject pressHint;

        [Inject] private SignalHolder _signalHolder;
        [Inject] private PlayerInput _playerInput;
        [Inject] private IVariablesResolver<PlayerVariableBlockerType, Action, Action> _playerVariables;
        [Inject] private MinigamesManager _minigamesManager;
        [Inject] private BaseMinigame[] _minigames;

        private float _signalPower;
        private BaseMinigame _lastMinigame;
        private DoorPasswordMinigame _doorMinigame;

        public event Action<float, float> OnSignalPowerChanged;

        private void Start()
        {
            Bind();
            _doorMinigame = FindFirstObjectByType<DoorPasswordMinigame>(FindObjectsInactive.Include);
        }

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

            _lastMinigame = _minigames.Except(new[] { _lastMinigame, _doorMinigame }).GetRandomElement();
            _minigamesManager.PlayMinigame(_lastMinigame, _signalHolder.GetCurrentSignal());
        }

        private void SetSignalPower(float power)
        {
            var temp = _signalPower;
            _signalPower = power;
            
            pressHint.SetActive(Mathf.Approximately(power, 1f));
            
            OnSignalPowerChanged?.Invoke(temp, power);
        }

        public float GetSignalPower01() => _signalPower;

        private void Bind() => _playerInput.OnSendSignalClicked += OnSendSignalClicked;

        private void Expose() => _playerInput.OnSendSignalClicked -= OnSendSignalClicked;
    }
}