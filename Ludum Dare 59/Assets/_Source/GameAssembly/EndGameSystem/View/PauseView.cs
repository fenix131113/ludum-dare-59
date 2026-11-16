using EndGameSystem;
using MiniGames;
using Player;
using Player.Data;
using Player.Variables;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;
using Utils.VariablesSystem;
using VContainer;

public class PauseView : MonoBehaviour
{
    [SerializeField] private int menuSceneIndex = 1;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button restartButton;

    [Inject] private IVariablesResolver<PlayerVariableBlockerType, Action, Action> _variablesResolver;
    [Inject] private MinigamesManager _minigamesManager;
    [Inject] private PlayerInput _playerInput;
    [Inject] private SceneLoader _sceneLoader;

    private readonly PlayerVariableBlocker _pauseBlocker = new(PlayerVariableBlockerType.ALL);


    private void Start() => Bind();

    private void OnDestroy() => Expose();

    private void OnRestartButtonClicked()
    {
        GameStateSaver.IsRestarted = true;
        TogglePause();
        _sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnContinueButtonClicked()
    {
        TogglePause();
    }

    private void OnMenuButtonClicked()
    {
        GameStateSaver.IsRestarted = false;
        TogglePause();
        _sceneLoader.LoadScene(menuSceneIndex);
    }

    private void OnPauseClick()
    {
        if(_minigamesManager.IsPlaying)
        {
            return;
        }
        TogglePause();
    }


    private void TogglePause()
    {
        if (!pausePanel.activeSelf)
        {
            pausePanel?.SetActive(true);
            _variablesResolver?.RegisterBlocker(_pauseBlocker);
            Time.timeScale = 0;
        }
        else
        {
            pausePanel?.SetActive(false);
            _pauseBlocker.Dispose();
            Time.timeScale = 1;
        }
    }

    private void Bind()
    {
        _playerInput.OnPauseClicked += OnPauseClick;
        _playerInput.OnRestartClicked += OnRestartButtonClicked;
        menuButton.onClick.AddListener(OnMenuButtonClicked);
        continueButton.onClick.AddListener(OnContinueButtonClicked);
        restartButton.onClick.AddListener(OnRestartButtonClicked);
    }
    private void Expose()
    {
        _playerInput.OnPauseClicked -= OnPauseClick;
        _playerInput.OnRestartClicked -= OnRestartButtonClicked;
        menuButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
    }
}
