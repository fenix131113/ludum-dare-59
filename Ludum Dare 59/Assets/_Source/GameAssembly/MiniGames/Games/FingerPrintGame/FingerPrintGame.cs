using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace MiniGames.Games.FingerPrintGame
{
    public class FingerPrintGame : BaseMinigame
    {
        [SerializeField] private GameObject gamePanel;
        [SerializeField] private Image rightAnswerImage;
        [SerializeField] private Button confirmButton;
        [SerializeField] private List<FingerButton> fingersButtons;
        [SerializeField] private Sprite[] variants;
        [SerializeField] private Sprite[] bigVariants;
        [SerializeField] private float scaleAnimTime = 0.3f;

        private int _rightAnswerIndex;
        private FingerButton _currentAnswerButton;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        public override void StartMinigame()
        {
            ResetGame();
            gamePanel.SetActive(true);
        }

        public override void EndMinigame()
        {
            gamePanel.SetActive(false);
        }

        public override void ResetGame()
        {
            Generate();
            confirmButton.interactable = false;
            _currentAnswerButton?.SetInteractable(true);
            _currentAnswerButton = null;
        }

        private void Generate()
        {
            var tempVariants = variants.ToList();
            var findTemp = variants.ToList();

            foreach (var t in fingersButtons)
            {
                var rndIndex = tempVariants.GetRandomIndex();
                t.Initialize(findTemp.IndexOf(tempVariants[rndIndex]), tempVariants[rndIndex]);
                tempVariants.RemoveAt(rndIndex);
            }

            _rightAnswerIndex = fingersButtons[fingersButtons.GetRandomIndex()].Index;
            rightAnswerImage.sprite = bigVariants[_rightAnswerIndex];
        }

        private void OnConfirmButtonClicked()
        {
            if (!_currentAnswerButton)
                return;

            if (_currentAnswerButton.Index == _rightAnswerIndex)
                InvokeGameEnded();
            else
            {
                _currentAnswerButton.selectedIconImage.gameObject.SetActive(false);
                _currentAnswerButton.gameObject.transform.DOScale(new Vector3(1, 1), scaleAnimTime);
                _currentAnswerButton.SetInteractable(true);
                _currentAnswerButton = null;
                confirmButton.interactable = false;
            }
        }

        private void OnFingerButtonClicked(FingerButton button)
        {
            _currentAnswerButton?.gameObject.transform.DOScale(new Vector3(1, 1), scaleAnimTime);
            _currentAnswerButton?.selectedIconImage.gameObject.SetActive(false);
            _currentAnswerButton?.SetInteractable(true);
            _currentAnswerButton = button;
            _currentAnswerButton.SetInteractable(false);
            _currentAnswerButton.selectedIconImage.gameObject.SetActive(true);
            _currentAnswerButton.gameObject.transform.DOScale(new Vector3(0.8f, 0.8f), scaleAnimTime);
            confirmButton.interactable = true;
        }

        private void Bind()
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);

            fingersButtons.ForEach(x => x.OnClick += OnFingerButtonClicked);
        }

        private void Expose()
        {
            confirmButton.onClick.RemoveAllListeners();

            fingersButtons.ForEach(x => x.OnClick -= OnFingerButtonClicked);
        }
    }
}