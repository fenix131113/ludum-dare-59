using System;
using MiniGames;
using MiniGames.Games.DoorPassword;
using Player.Data;
using UnityEngine;
using Utils.VariablesSystem;
using VContainer;

namespace InteractionSystem
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpriteRenderer doorRenderer;
        [SerializeField] private Collider2D doorCollider;
        [SerializeField] private Sprite openedSprite;
        
        [Inject] private AstarPath _path;
        [Inject] private MinigamesManager _minigamesManager;
        
        private DoorPasswordMinigame  _passwordGame;
        private bool _opened;

        private void Start()
        {
            _passwordGame = FindFirstObjectByType<DoorPasswordMinigame>(FindObjectsInactive.Include);
        }

        public bool CanInteract() => !_opened;

        public void Interact()
        {
            _minigamesManager.PlayMinigame(_passwordGame);
            Bind();
        }
        
        public void Open()
        {
            doorRenderer.sprite = openedSprite;
            doorCollider.enabled = false;
            _opened = true;
            _path.Scan();
        }

        private void OnMinigameStateChanged()
        {
            if(_minigamesManager.IsPlaying)
                return;
            
            Open();
            Expose();
        }

        private void Bind()
        {
            _minigamesManager.OnMinigameStateChanged += OnMinigameStateChanged;
        }

        private void Expose()
        {
            _minigamesManager.OnMinigameStateChanged -= OnMinigameStateChanged;
        }
    }
}