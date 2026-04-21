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
        [SerializeField] private float minNextSignalSpawnDistance;
        [SerializeField] private float maxNextSignalSpawnDistance = float.MaxValue;

        private SignalEmitter _currentSignal;
        private Transform _lastSpawnPoint;

        public event Action OnSignalSpawned;
        public event Action OnSignalSent;

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
            var availableSpawnPoints = signalSpawnPoints.Where(x => x).ToArray();

            if (availableSpawnPoints.Length < 2)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Not enough signal spawn points!");
#endif
                return;
            }

            if (_currentSignal)
                DestroySignal(_currentSignal);

            _lastSpawnPoint = GetNextSpawnPoint(availableSpawnPoints);

            if (!_lastSpawnPoint)
                return;

            InitializeSignal(Instantiate(emitterPrefab, _lastSpawnPoint.position, Quaternion.identity));
        }

        private Transform GetNextSpawnPoint(IEnumerable<Transform> availableSpawnPoints)
        {
            if (!_lastSpawnPoint)
                return availableSpawnPoints.GetRandomElement();

            var minDistance = minNextSignalSpawnDistance > 0 ? minNextSignalSpawnDistance : 0;
            var maxDistance = maxNextSignalSpawnDistance > 0 ? maxNextSignalSpawnDistance : float.MaxValue;

            if (maxDistance < minDistance)
                maxDistance = minDistance;

            var fallbackSpawnPoints = availableSpawnPoints.Except(new[] { _lastSpawnPoint }).ToArray();
            var suitableSpawnPoints = fallbackSpawnPoints
                .Where(x =>
                {
                    var distance = Vector2.Distance(_lastSpawnPoint.position, x.position);
                    return distance >= minDistance && distance <= maxDistance;
                })
                .ToArray();

            return suitableSpawnPoints.Length > 0
                ? suitableSpawnPoints.GetRandomElement()
                : fallbackSpawnPoints.GetRandomElement();
        }

        private void OnCurrentSignalSent()
        {
            DestroySignal(_currentSignal);
            SpawnNewSignal();
            OnSignalSent?.Invoke();
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