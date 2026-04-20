using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace MiniGames.Games.DoorPassword
{
    public class DoorPasswordMinigame : BaseMinigame
    {
        [SerializeField] private GameObject minigamePanel;
        [SerializeField] private DoorPasswordButton[] passwordButtons;
        [SerializeField] private TMP_Text passwordHintText;
        [SerializeField] private TMP_Text currentInputText;
        [SerializeField] private Image monitorImage;
        [SerializeField] private Sprite defaultMonitorSprite;
        [SerializeField] private Sprite successMonitorSprite;
        [SerializeField] private int passwordLength = 4;

        private readonly List<int> _password = new();
                                                                 private readonly List<int> _currentInput = new();
        private bool _isBind;

        private void Start() => Bind();

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
            ResetMonitorState();
            GeneratePassword();
            ResetInput();
            ResetButtonsState();
            SetButtonsInteractable(true);
            RedrawHint();
            RedrawInput();
        }

        private void GeneratePassword()
        {
            _password.Clear();

            if (passwordLength <= 0)
                return;
            
            var possibleValues = passwordButtons.Where(x => x).Select(x => x.Value).ToList();

            if (possibleValues.Count == 0)
                return;

            for (var index = 0; index < passwordLength; index++)
            {
                var rnd = possibleValues.GetRandomIndex();
                _password.Add(possibleValues[rnd]);
                possibleValues.RemoveAt(rnd);
            }
        }

        private void OnPasswordButtonClicked(int value)
        {
            if (_password.Count == 0)
                return;

            _currentInput.Add(value);
            RedrawInput();

            if (_currentInput.Count < _password.Count)
                return;

            if (IsPasswordCorrect())
            {
                SetButtonsInteractable(false);
                SetMonitorSuccessState();
                InvokeGameEnded();
                return;
            }

            ResetInput();
            ResetButtonsState();
            RedrawInput();
        }

        private bool IsPasswordCorrect()
        {
            if (_currentInput.Count != _password.Count)
                return false;

            for (var index = _password.Count - 1; index >= 0; index--)
            {
                if (_password[index] != _currentInput[index])
                    return false;
            }

            return true;
        }

        private void RedrawHint()
        {
            if (passwordHintText)
                passwordHintText.text = string.Join(" ", _password);
        }

        private void RedrawInput()
        {
            if (!currentInputText)
                return;

            var symbols = new string[Mathf.Max(passwordLength, 0)];

            for (var index = 0; index < symbols.Length; index++)
                symbols[index] = index < _currentInput.Count ? _currentInput[index].ToString() : "_";

            currentInputText.text = string.Join(" ", symbols);
        }

        private void SetMonitorSuccessState()
        {
            if (monitorImage && successMonitorSprite)
                monitorImage.sprite = successMonitorSprite;
        }

        private void ResetMonitorState()
        {
            if (monitorImage && defaultMonitorSprite)
                monitorImage.sprite = defaultMonitorSprite;
        }

        private void ResetButtonsState()
        {
            foreach (var passwordButton in passwordButtons)
            {
                if (!passwordButton)
                    continue;

                passwordButton.ResetState();
            }
        }

        private void SetButtonsInteractable(bool isInteractable)
        {
            foreach (var passwordButton in passwordButtons)
            {
                if (!passwordButton)
                    continue;

                passwordButton.SetInteractable(isInteractable);
            }
        }

        private void ResetInput()
        {
            _currentInput.Clear();
        }

        private void Bind()
        {
            if (_isBind)
                return;
            
            foreach (var passwordButton in passwordButtons)
                passwordButton.OnClicked += OnPasswordButtonClicked;

            _isBind = true;
        }

        private void Expose()
        {
            if (!_isBind)
                return;

            foreach (var passwordButton in passwordButtons)
                passwordButton.OnClicked -= OnPasswordButtonClicked;

            _isBind = false;
        }
    }
}
