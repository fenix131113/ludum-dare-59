using System.Linq;
using UnityEngine;
using Utils.Extensions;
using Random = UnityEngine.Random;

namespace MiniGames.Games.RepeatPictures
{
    public class RepeatSymbolsMinigame : BaseMinigame
    {
        [SerializeField] private GameObject minigamePanel;
        [SerializeField] private SymbolCell[] sourceCells;
        [SerializeField] private SymbolCell[] answerCells;
        [SerializeField] private int maxValue;

        private int[] _targetValues;
        private bool _isBind;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        public override void StartMinigame()
        {
            minigamePanel.SetActive(true);
            ResetGame();
        }

        public override void EndMinigame()
        {
            minigamePanel.SetActive(false);
        }

        public override void ResetGame()
        {
            _targetValues = new int[answerCells.Length];

            GenerateTargetValues();
            ResetAnswerCells();
            ResetSourceCells();
        }

        private void GenerateTargetValues()
        {
            var generated = false;

            for (var index = 0; index < answerCells.Length; index++)
            {
                var value = Random.Range(1, maxValue + 1);
                _targetValues[index] = value;

                if (value != 1)
                    generated = true;
            }

            if (generated)
                return;

            var randomIndex = answerCells.GetRandomIndex();
            _targetValues[randomIndex] = Random.Range(2, maxValue + 1);
        }

        private void ResetSourceCells()
        {
            for (var index = 0; index < sourceCells.Length; index++)
                sourceCells[index].SetState(_targetValues[index], maxValue, false);
        }

        private void ResetAnswerCells()
        {
            foreach (var cell in answerCells)
                cell.SetState(1, maxValue, true);
        }

        private void OnAnswerCellClicked(SymbolCell _)
        {
            CheckForWinning();
        }

        private void CheckForWinning()
        {
            if (answerCells
                .Where((answerCell, index) => !answerCell || answerCell.CurrentSymbol != _targetValues[index]).Any())
                return;

            SetAnswerCellsInteractable(false);
            InvokeGameEnded();
        }

        private void SetAnswerCellsInteractable(bool isInteractable)
        {
            foreach (var answerCell in answerCells)
            {
                if (!answerCell)
                    continue;

                answerCell.SetInteractable(isInteractable);
            }
        }

        private void Bind()
        {
            if (_isBind)
                return;

            foreach (var answerCell in answerCells)
                answerCell.OnClicked += OnAnswerCellClicked;

            _isBind = true;
        }

        private void Expose()
        {
            if (!_isBind)
                return;

            foreach (var answerCell in answerCells)
                answerCell.OnClicked -= OnAnswerCellClicked;

            _isBind = false;
        }
    }
}