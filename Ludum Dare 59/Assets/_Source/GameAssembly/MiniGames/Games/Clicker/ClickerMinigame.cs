using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.Games.Clicker
{
    public sealed class ClickerMinigame : BaseMinigame
    {
        [SerializeField] private int needClicks;
        [SerializeField] private GameObject clickerPanel;
        [SerializeField] private ClickerButton clickerButton;
        [SerializeField] private TMP_Text clicksCounter;
        [SerializeField] private Image progressFill;

        public int Clicks { get; private set; }

        private bool _isBind;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void OnClickerClicked()
        {
            Clicks++;
            Redraw();
            CheckForWinning();
        }

        private void Redraw()
        {
            if(clicksCounter)
                clicksCounter.text = Clicks.ToString();
            
            if(progressFill)
                progressFill.fillAmount = (float)Clicks / needClicks;
        }

        private void CheckForWinning()
        {
            if (Clicks >= needClicks)
            {
                clickerButton.SetInteractable(false);
                InvokeGameEnded();
            }
        }

        public override void StartMinigame()
        {
            ResetGame();
            clickerPanel.gameObject.SetActive(true);
        }

        public override void EndMinigame()
        {
            clickerPanel.gameObject.SetActive(false);
        }

        public override void ResetGame()
        {
            Clicks = 0;
            clickerButton.SetInteractable(true);

            Redraw();
        }

        private void Bind()
        {
            clickerButton.OnClicked += OnClickerClicked;
            _isBind = true;
        }

        private void Expose()
        {
            if (!_isBind)
                return;

            clickerButton.OnClicked -= OnClickerClicked;
        }
    }
}