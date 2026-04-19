using System.Linq;
using TMPro;
using UnityEngine;
using Utils.Extensions;

namespace MiniGames.Games.FilesDrag
{
    public class FilesDragMinigame : BaseMinigame
    {
        [SerializeField] private GameObject minigamePanel;
        [SerializeField] private RectTransform dropZone;
        [SerializeField] private DragFileItem[] fileItems;
        [SerializeField] private TMP_Text filesCounter;
        [SerializeField] private int fileCount = 8;

        private bool _isBind;
        private int _currentTransferredFiles;
        private int _currentNeedFilesCount;

        private void Start()
        {
            Bind();
        }

        private void OnDestroy() => Expose();

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
            _currentTransferredFiles = 0;
            _currentNeedFilesCount = fileCount;
            
            RedrawCounter();
            ResetFiles();
        }

        private void ResetFiles()
        {
            foreach (var item in fileItems)
            {
                if (!item)
                    continue;

                item.SetDropZone(dropZone);
                item.gameObject.SetActive(false);
            }

            var tempFiles = fileItems.ToList();

            var i = 0;
            while (i < _currentNeedFilesCount)
            {
                var index = tempFiles.GetRandomIndex();
                tempFiles[index].ResetItem();
                tempFiles.RemoveAt(index);
                i++;
            }
        }

        private void OnFileDropped(DragFileItem _)
        {
            _currentTransferredFiles++;
            RedrawCounter();

            if (_currentTransferredFiles >= _currentNeedFilesCount)
                InvokeGameEnded();
        }

        private void RedrawCounter()
        {
            if (filesCounter)
                filesCounter.text = $"{_currentTransferredFiles}/{_currentNeedFilesCount}";
        }

        private void Bind()
        {
            if (_isBind)
                return;

            foreach (var fileItem in fileItems)
                fileItem.OnFileDropped += OnFileDropped;

            _isBind = true;
        }

        private void Expose()
        {
            if (!_isBind)
                return;

            foreach (var fileItem in fileItems)
                fileItem.OnFileDropped -= OnFileDropped;

            _isBind = false;
        }
    }
}
