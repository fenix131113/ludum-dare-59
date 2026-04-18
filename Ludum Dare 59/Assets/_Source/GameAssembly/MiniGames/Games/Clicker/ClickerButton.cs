using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGames.Games.Clicker
{
    public class ClickerButton : MonoBehaviour, IPointerClickHandler //TODO: Add animation
    {
        [field: SerializeField] public bool Interactable { get; private set; } = true;
        
        public event Action OnClicked;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if(!Interactable)
                return;
            
            OnClicked?.Invoke();   
        }
        
        public void SetInteractable(bool interactable) =>  Interactable = interactable;
    }
}