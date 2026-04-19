using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.Games.RepeatPictures
{
    public class SymbolCell : MonoBehaviour
    {
        [SerializeField] private Button actionButton;
        [SerializeField] private TMP_Text symbolText;

        private int _cellMaxValue = 1;
        private bool _isBind;

        public int CurrentSymbol { get; private set; }

        public event Action<SymbolCell> OnClicked;

        private void Start()
        {
            Bind();
            Redraw();
        }

        private void OnDestroy() => Expose();

        private void OnButtonClicked()
        {
            CurrentSymbol++;

            if (CurrentSymbol > _cellMaxValue)
                CurrentSymbol = 1;

            Redraw();
            OnClicked?.Invoke(this);
        }

        private int NormalizeValue(int value)
        {
            if (value <= 0)
                return 1;

            if (value > _cellMaxValue)
                return (value - 1) % _cellMaxValue + 1;

            return value;
        }

        private void Redraw()
        {
            if (symbolText)
                symbolText.text = CurrentSymbol.ToString();
        }
        
        public void SetState(int value, int symbolsCount, bool isInteractable)
        {
            _cellMaxValue = symbolsCount;
            CurrentSymbol = NormalizeValue(value);

            if (actionButton)
                actionButton.interactable = isInteractable;

            Redraw();
        }

        public void SetInteractable(bool isInteractable)
        {
            if (actionButton)
                actionButton.interactable = isInteractable;
        }

        private void Bind()
        {
            if (_isBind || !actionButton)
                return;

            actionButton.onClick.AddListener(OnButtonClicked);
            _isBind = true;
        }

        private void Expose()
        {
            if (!_isBind || !actionButton)
                return;

            actionButton.onClick.RemoveAllListeners();
            _isBind = false;
        }
    }
}
