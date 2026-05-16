using System;
using ComicsSystem;
using LevelsSystem.Data;
using Player.Data;
using Player.Variables;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils.VariablesSystem;
using VContainer;

namespace EndGameSystem.View
{
    public class EndGameView : MonoBehaviour
    {
        [SerializeField] private int menuSceneIndex = 1;
        [SerializeField] private GameObject endPanel;
        [SerializeField] private Button menuButton;
        [SerializeField] private TMP_Text resultLabel;
        [SerializeField] private Comics endComics;
        [SerializeField] private Comics winComics;
        [SerializeField] private string winCaption = "You win!";
        [SerializeField] private string looseCaption = "You loose!";
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button restartButton;

        [Inject] private EndGame _endGame;
        [Inject] private IVariablesResolver<PlayerVariableBlockerType, Action, Action>  _variablesResolver;
        [Inject] private LevelDataSO _levelData;

        private readonly PlayerVariableBlocker _endBlocker = new(PlayerVariableBlockerType.ALL);

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void OnGameEnded(bool isWin)
        {
            _variablesResolver.RegisterBlocker(_endBlocker);
            
            if(isWin)
                Win();
            else
                Loose();
            
            if (_levelData.NextLevelIndex == -1)
                            nextLevelButton.gameObject.SetActive(false);
            
            endPanel.SetActive(true);
        }

        private void OnMenuButtonClicked() => SceneManager.LoadScene(menuSceneIndex);

        private void OnNextLevelButtonClicked() => SceneManager.LoadScene(_levelData.NextLevelIndex);

        private void OnRestartButtonClicked() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        private void Win()
        {
            resultLabel.text = winCaption;
            winComics?.gameObject.SetActive(true);
            endComics.gameObject.SetActive(false);
        }

        private void Loose()
        {
            resultLabel.text = looseCaption;
            endComics.gameObject.SetActive(true);
            nextLevelButton.gameObject.SetActive(false);
        }
        
        private void Bind()
        {
            _endGame.OnGameEnded += OnGameEnded;
            menuButton.onClick.AddListener(OnMenuButtonClicked);
            nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        private void Expose()
        {
            _endGame.OnGameEnded -= OnGameEnded;
            menuButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.RemoveAllListeners();
            restartButton.onClick.RemoveAllListeners();
        }
    }
}