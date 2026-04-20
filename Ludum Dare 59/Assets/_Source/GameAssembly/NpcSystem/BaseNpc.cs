using Pathfinding;
using UnityEngine;

namespace NpcSystem
{
    public abstract class BaseNpc : MonoBehaviour
    {
        [SerializeField] protected AIPath aiPath;
        [SerializeField] protected Transform[] movePoints;
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected float pointReachDistance;
        [SerializeField] protected float destinationUpdateDistance = 0.1f;
        [SerializeField] protected bool constrainInsideGraph = true;
        [SerializeField] protected bool useCyclePatrol = true;

        protected int _movePointIndex;
        protected bool _isMoveForward = true;
        protected Vector2 _lastDestination;
        protected bool _hasDestination;

        protected float MoveSpeed => moveSpeed;

        private void Awake()
        {
            if (!aiPath)
                return;

            aiPath.maxSpeed = moveSpeed;
            aiPath.constrainInsideGraph = constrainInsideGraph;
        }

        private void Update() => Tick();

        protected abstract void Tick();

        protected virtual bool Patrol()
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

        protected virtual void MoveToPoint(Vector2 point, float speedMultiplier = 1f)
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

        protected virtual void StopMoving()
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
