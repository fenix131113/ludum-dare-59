using ObstacleSystem;
using UnityEngine;

namespace NpcSystem
{
    public class BananaThrower : MonoBehaviour
    {
        [SerializeField] private MonkeyNpc monkey;
        [SerializeField] private Banana bananaPrefab;
        [SerializeField] private Transform throwPoint;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void OnBananaThrowRequested(Vector2 targetPos) =>
            Instantiate(bananaPrefab, throwPoint.position, Quaternion.identity).Initialize(targetPos);

        private void Bind() => monkey.OnBananaThrowRequested += OnBananaThrowRequested;

        private void Expose() => monkey.OnBananaThrowRequested -= OnBananaThrowRequested;
    }
}