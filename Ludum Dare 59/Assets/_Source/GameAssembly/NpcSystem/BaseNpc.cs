using Pathfinding;
using UnityEngine;

namespace NpcSystem
{
    public abstract class BaseNpc : MonoBehaviour
    {
        [SerializeField] private AIPath aiPath;
        [SerializeField] private Transform[] movePoints;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float pointReachDistance;
        [SerializeField] private float destinationUpdateDistance = 0.1f;
        [SerializeField] private bool useCyclePatrol = true;

        private int _movePointIndex;
        private bool _isMoveForward = true;
        private Vector2 _lastDestination;
        private bool _hasDestination;

        protected float MoveSpeed => moveSpeed;

        private void Awake()
        {
            if (!aiPath)
                return;

            aiPath.maxSpeed = moveSpeed;
        }

        private void Update() => Tick();

        protected abstract void Tick();

        protected bool Patrol()
        {
            if (!TryGetCurrentPoint(out var point))
            {
                StopMoving();
                return false;
            }

            MoveToPoint(point);

            if (IsPointReached(point, pointReachDistance))
                SelectNextMovePoint();

            return true;
        }

        protected void MoveToPoint(Vector2 point, float speedMultiplier = 1f)
        {
            if (!aiPath)
                return;

            aiPath.maxSpeed = moveSpeed * speedMultiplier;
            aiPath.isStopped = false;

            if (_hasDestination && Vector2.Distance(_lastDestination, point) <= destinationUpdateDistance)
                return;

            _lastDestination = point;
            _hasDestination = true;
            aiPath.destination = point;
            aiPath.SearchPath();
        }

        protected void StopMoving()
        {
            if (!aiPath)
                return;

            aiPath.maxSpeed = moveSpeed;
            aiPath.isStopped = true;
            _hasDestination = false;
        }

        protected bool IsPointReached(Vector2 point, float distance) =>
            Vector2.Distance(transform.position, point) <= distance;

        private bool TryGetCurrentPoint(out Vector2 point)
        {
            point = default;

            if (movePoints == null || movePoints.Length == 0)
                return false;

            for (var i = 0; i < movePoints.Length; i++)
            {
                if (movePoints[_movePointIndex])
                {
                    point = movePoints[_movePointIndex].position;
                    return true;
                }

                SelectNextMovePoint();
            }

            return false;
        }

        private void SelectNextMovePoint()
        {
            if (movePoints is not { Length: > 1 })
                return;

            if (useCyclePatrol)
            {
                _movePointIndex++;

                if (_movePointIndex >= movePoints.Length)
                    _movePointIndex = 0;

                return;
            }

            if (_isMoveForward)
            {
                _movePointIndex++;

                if (_movePointIndex >= movePoints.Length - 1)
                    _isMoveForward = false;
            }
            else
            {
                _movePointIndex--;

                if (_movePointIndex <= 0)
                    _isMoveForward = true;
            }
        }
    }
}
