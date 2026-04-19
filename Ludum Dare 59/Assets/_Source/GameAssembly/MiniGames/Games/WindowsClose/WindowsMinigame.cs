using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MiniGames.Games.WindowsClose
{
    public class WindowsMinigame : BaseMinigame
    {
        [SerializeField] private GameObject minigamePanel;
        [SerializeField] private Transform windowsParent;
        [SerializeField] private Window windowPrefab;
        [SerializeField] private Button sendButton;
        [SerializeField] private int minCount;
        [SerializeField] private int maxCount;

        private readonly List<Window> _spawnedWindows = new();
        private bool _isBind;
        private int _currentClosedCount;
        private int _currentNeedCloseCount;

        private void Start()
        {
            Bind();
        }

        private void OnDestroy()
        {
            Expose();
            ExposeWindows();
        }

        public override void StartMinigame()
        {
            ResetGame();
            minigamePanel.SetActive(true);
        }

        public override void EndMinigame()
        {
            minigamePanel.SetActive(false);
        }

        public override void ResetGame()
        {
            _currentNeedCloseCount = Random.Range(minCount, maxCount + 1);
            _currentClosedCount = 0;
            sendButton.interactable = false;
            SpawnWindows();
            RandomizeWindowsPosition();
        }

        private void SpawnWindows()
        {
            ExposeWindows();

            for (var i = _spawnedWindows.Count; i < _currentNeedCloseCount; i++)
                _spawnedWindows.Add(Instantiate(windowPrefab, windowsParent));

            _spawnedWindows.ForEach(x =>
            {
                x.ResetWindow();
                x.gameObject.SetActive(true);
            });

            BindWindows();
        }

        private void OnSendButtonClicked()
        {
            if (_currentClosedCount >= _currentNeedCloseCount)
                InvokeGameEnded();
        }

        private void RandomizeWindowsPosition()
        {
            var parentRect = windowsParent as RectTransform;
            if (!parentRect)
                return;

            var parentBounds = parentRect.rect;

            foreach (var window in _spawnedWindows.Where(window => window && window.gameObject.activeSelf))
            {
                if (!window.TryGetComponent<RectTransform>(out var windowRect))
                    continue;

                var windowWidth = windowRect.rect.width * windowRect.localScale.x;
                var windowHeight = windowRect.rect.height * windowRect.localScale.y;
                var pivot = windowRect.pivot;

                var minX = parentBounds.xMin + windowWidth * pivot.x;
                var maxX = parentBounds.xMax - windowWidth * (1f - pivot.x);
                var minY = parentBounds.yMin + windowHeight * pivot.y;
                var maxY = parentBounds.yMax - windowHeight * (1f - pivot.y);

                var randomX = minX > maxX ? (minX + maxX) * 0.5f : Random.Range(minX, maxX);
                var randomY = minY > maxY ? (minY + maxY) * 0.5f : Random.Range(minY, maxY);

                var currentLocalPosition = windowRect.localPosition;
                windowRect.localPosition = new Vector3(randomX, randomY, currentLocalPosition.z);
            }
        }

        private void OnWindowClosed()
        {
            _currentClosedCount++;

            if (_currentClosedCount >= _currentNeedCloseCount)
                sendButton.interactable = true;
        }

        private void BindWindows()
        {
            if (_isBind)
                return;

            _spawnedWindows.ForEach(x => x.OnClose += OnWindowClosed);

            if (_spawnedWindows.Count > 0)
                _isBind = true;
        }

        private void ExposeWindows()
        {
            if (!_isBind)
                return;

            _spawnedWindows.ForEach(x => x.OnClose -= OnWindowClosed);

            _isBind = false;
        }

        private void Bind()
        {
            sendButton.onClick.AddListener(OnSendButtonClicked);
        }

        private void Expose()
        {
            sendButton.onClick.RemoveAllListeners();
        }
    }
}