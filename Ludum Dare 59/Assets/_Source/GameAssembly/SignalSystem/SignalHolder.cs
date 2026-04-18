using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Extensions;

namespace SignalSystem
{
    public class SignalHolder : MonoBehaviour
    {
        [SerializeField] private List<Transform> signalSpawnPoints;
        [SerializeField] private SignalEmitter emitterPrefab;

        private SignalEmitter _currentSignal;
        private Transform _lastSpawnPoint;

        public event Action OnSignalSpawned;

        private void OnDestroy()
        {
            ExposeSignal(_currentSignal);
            OnSignalSpawned = null;
        }

        private void Start()
        {
            SpawnNewSignal();
        }

        private void SpawnNewSignal()
        {
            if (signalSpawnPoints.Count < 2)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Not enough signal spawn points!");
#endif
                return;
            }

            if (_currentSignal)
                DestroySignal(_currentSignal);

            _lastSpawnPoint = signalSpawnPoints.Except(new[] { _lastSpawnPoint }).GetRandomElement();
            InitializeSignal(Instantiate(emitterPrefab, _lastSpawnPoint.position, Quaternion.identity));
        }

        private void OnCurrentSignalSent()
        {
            DestroySignal(_currentSignal);
            SpawnNewSignal();
        }

        private void InitializeSignal(SignalEmitter signal)
        {
            ExposeSignal(_currentSignal);
            _currentSignal = signal;
            BindSignal(_currentSignal);
            OnSignalSpawned?.Invoke();
        }

        private void DestroySignal(SignalEmitter signal)
        {
            ExposeSignal(signal);
            Destroy(signal.gameObject);
        }

        public SignalEmitter GetCurrentSignal() => _currentSignal;

        private void BindSignal(SignalEmitter signal)
        {
            if (signal)
                signal.OnSignalSent += OnCurrentSignalSent;
        }

        private void ExposeSignal(SignalEmitter signal)
        {
            if (signal)
                signal.OnSignalSent -= OnCurrentSignalSent;
        }
    }
}