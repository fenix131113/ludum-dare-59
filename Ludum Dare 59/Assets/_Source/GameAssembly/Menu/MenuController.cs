using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button backFromSettingButton;
        [SerializeField] private Button backFromPlayButton;
        [SerializeField] private RectTransform mainPanel;
        [SerializeField] private RectTransform playPanel;
        [SerializeField] private RectTransform settingsPanel;
        [SerializeField] private GameObject touchBlocker;
        [SerializeField] private Transform playerCamera;
        [SerializeField] private float moveTime;
        [SerializeField] private Ease moveEase;
        [SerializeField] private float cameraSettingsX;
        [SerializeField] private float cameraPlayX;

        private Vector2 _startMainPanelPos;
        private Vector2 _startPlayPanelPos;
        private Vector2 _startSettingsPanelPos;

        private void Start()
        {
            Bind();
            
            _startPlayPanelPos = playPanel.anchoredPosition;
            _startSettingsPanelPos = settingsPanel.anchoredPosition;
            _startMainPanelPos =  mainPanel.anchoredPosition;
        }

        private void OnDestroy()
        {
            Expose();
        }

        private void OnPlayButtonClicked()
        {
            touchBlocker.SetActive(true);

            playerCamera.DOMoveX(cameraPlayX, moveTime).SetEase(moveEase);
            playPanel.DOAnchorPos(_startMainPanelPos, moveTime).SetEase(moveEase);
            mainPanel.DOAnchorPos(_startSettingsPanelPos, moveTime).SetEase(moveEase)
                .OnComplete(() => touchBlocker.SetActive(false));
        }

        private void OnSettingsButtonClicked()
        {
            touchBlocker.SetActive(true);

            playerCamera.DOMoveX(cameraSettingsX, moveTime).SetEase(moveEase);
            settingsPanel.DOAnchorPos(_startMainPanelPos, moveTime).SetEase(moveEase);
            mainPanel.DOAnchorPos(_startPlayPanelPos, moveTime).SetEase(moveEase)
                .OnComplete(() => touchBlocker.SetActive(false));
        }
        
        private void OnBackFromSettingsButtonClicked()
        {
            touchBlocker.SetActive(true);

            playerCamera.DOMoveX(0, moveTime).SetEase(moveEase);
            settingsPanel.DOAnchorPos(_startSettingsPanelPos, moveTime).SetEase(moveEase);
            mainPanel.DOAnchorPos(_startMainPanelPos, moveTime).SetEase(moveEase)
                .OnComplete(() => touchBlocker.SetActive(false));
        }
        
        private void OnBackFromPlayButtonClicked()
        {
            touchBlocker.SetActive(true);

            playerCamera.DOMoveX(0, moveTime).SetEase(moveEase);
            playPanel.DOAnchorPos(_startPlayPanelPos, moveTime).SetEase(moveEase);
            mainPanel.DOAnchorPos(_startMainPanelPos, moveTime).SetEase(moveEase)
                .OnComplete(() => touchBlocker.SetActive(false));
        }

        private void Bind()
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
            settingButton.onClick.AddListener(OnSettingsButtonClicked);
            backFromPlayButton.onClick.AddListener(OnBackFromPlayButtonClicked);
            backFromSettingButton.onClick.AddListener(OnBackFromSettingsButtonClicked);
        }

        private void Expose()
        {
            playButton.onClick.RemoveAllListeners();
            settingButton.onClick.RemoveAllListeners();
            backFromPlayButton.onClick.RemoveAllListeners();
            backFromSettingButton.onClick.RemoveAllListeners();
        }
    }
}