using Coffee.UIExtensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;

namespace MiniGames.Games.Clicker
{
    public sealed class ClickerMinigame : BaseMinigame
    {
        [SerializeField] private int needClicks;
        [SerializeField] private GameObject clickerPanel;
        [SerializeField] private ClickerButton clickerButton;
        [SerializeField] private TMP_Text clicksCounter;
        [SerializeField] private Image progressFill;
        [SerializeField] private TempParticleSource filesParticles;
        [SerializeField] private UIParticle particlesParent;
        [SerializeField] private float popSize;
        [SerializeField] private float popTime;
        [SerializeField] private Ease popEase;

        public int Clicks { get; private set; }

        private bool _isBind;
        private Tween _popTween;
        private Vector3 _clickerStartScale;

        private void Start()
        {
            Bind();
            _clickerStartScale = clickerButton.transform.localScale;
        }

        private void OnDestroy() => Expose();

        private void OnClickerClicked()
        {
            _popTween?.Kill();
            _popTween = clickerButton.transform.DOScale(_clickerStartScale * popSize, popTime / 2f)
                .SetEase(popEase).OnComplete(() =>
                    clickerButton.transform.DOScale(_clickerStartScale, popTime / 2f).SetEase(popEase));

            Instantiate(filesParticles, Camera.main!.ScreenToWorldPoint(Mouse.current.position.ReadValue()),
                filesParticles.transform.rotation, particlesParent.transform);
            
            particlesParent.RefreshParticles();

            Clicks++;
            Redraw();
            CheckForWinning();
        }

        private void Redraw()
        {
            if (clicksCounter)
                clicksCounter.text = (needClicks - Clicks).ToString();

            if (progressFill)
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