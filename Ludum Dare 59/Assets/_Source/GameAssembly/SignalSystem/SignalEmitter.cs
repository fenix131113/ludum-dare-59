using System;
using UnityEngine;
using Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SignalSystem
{
    public class SignalEmitter : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D signalCollider;
        [SerializeField] private float signalRadius;

        public event Action OnSignalSent;

        private void Start() => signalCollider.radius = signalRadius;

        public void SendSignal()
        {
            OnSignalSent?.Invoke();
        }
        
        public float GetSignalRadius() => signalRadius;

        private void OnDrawGizmosSelected()
        {
            if(!signalCollider)
                return;

#if UNITY_EDITOR
            Handles.color = new Color(0f, 1f, 0f, 0.2f);
            Handles.DrawSolidDisc(transform.position, Vector3.forward, signalRadius);
            Handles.color = Color.green;
            Handles.DrawWireDisc(transform.position, Vector3.forward, signalRadius);
#endif
        }
    }
}
