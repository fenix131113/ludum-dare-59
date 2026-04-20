using System;
using ComicsSystem;
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
        [SerializeField] private string winCaption = "You win!";
        [SerializeField] private string looseCaption = "You loose!";

        [Inject] private EndGame _endGame;
        [Inject] private IVariablesResolver<PlayerVariableBlockerType, Action, Action>  _variablesResolver;

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
            
            endPanel.SetActive(true);
        }

        private void OnMenuButtonClicked()
        {
            SceneManager.LoadScene(menuSceneIndex);
        }

        private void Win()
        {
            resultLabel.text = winCaption;
            endComics.gameObject.SetActive(false);
        }

        private void Loose()
        {
            resultLabel.text = looseCaption;
            endComics.gameObject.SetActive(true);
        }
        
        private void Bind()
        {
            _endGame.OnGameEnded += OnGameEnded;
            menuButton.onClick.AddListener(OnMenuButtonClicked);
        }

        private void Expose()
        {
            _endGame.OnGameEnded -= OnGameEnded;
            menuButton.onClick.RemoveAllListeners();
        }
    }
}