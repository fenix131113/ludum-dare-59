using Player;
using UnityEngine;
using Utils;
using VContainer;

namespace InteractionSystem
{
    public class InteractionTracker : MonoBehaviour
    {
        [SerializeField] private LayerMask interactionLayers;
        [SerializeField] private GameObject interactionHint;
        
        [Inject] private PlayerInput _playerInput;
        
        private IInteractable _currentInteractable;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void Update()
        {
            if(_currentInteractable == null ||  !_currentInteractable.CanInteract())
            {
                interactionHint.SetActive(false);
                return;
            }
            
            interactionHint.SetActive(true);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!LayerService.CheckLayersEquality(other.gameObject.layer, interactionLayers))
                return;

            if(!other.gameObject.TryGetComponent<IInteractable>(out var interactable))
                return;
            
            if(!interactable.CanInteract())
                return;
            
            _currentInteractable =  interactable;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(!LayerService.CheckLayersEquality(other.gameObject.layer, interactionLayers))
                return;
            
            if(!other.gameObject.TryGetComponent<IInteractable>(out var interactable))
                return;

            if (_currentInteractable == interactable)
                _currentInteractable = null;
        }

        private void OnInteractionClicked()
        {
            if(_currentInteractable == null || !_currentInteractable.CanInteract())
                return;
            
            _currentInteractable.Interact();
        }

        private void Bind() => _playerInput.OnSendSignalClicked += OnInteractionClicked;

        private void Expose() => _playerInput.OnSendSignalClicked -= OnInteractionClicked;
    }
}