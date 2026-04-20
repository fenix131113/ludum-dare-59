using System;
using Player;
using UnityEngine;
using Utils;
using VContainer;
using Random = UnityEngine.Random;

namespace NpcSystem
{
    public class MonkeyNpc : BaseNpc
    {
        private static readonly int _moveX = Animator.StringToHash("MoveX");
        private static readonly int _moveY = Animator.StringToHash("MoveY");
        private static readonly int _isMoving = Animator.StringToHash("IsMoving");
        private static readonly int _throw = Animator.StringToHash("Throw");
        private static readonly int _throwX = Animator.StringToHash("ThrowX");
        private static readonly int _throwY = Animator.StringToHash("ThrowY");

        [SerializeField] private LayerMask playerLayers;
        [SerializeField] private LayerMask sightBlockLayers;
        [SerializeField] private float viewDistance;
        [SerializeField] private float keepDistance;
        [SerializeField] private float retreatDistance;
        [SerializeField] private float safeDistance;
        [SerializeField] private float searchPointReachDistance;
        [SerializeField] private float searchTimeout = 2f;
        [SerializeField] private float retreatPointReachDistance;
        [SerializeField] private float lostSightDelay = 0.2f;
        [SerializeField] private float retreatSpeedMultiplier = 1.4f;
        [SerializeField] private float throwCooldown;
        [SerializeField] private float throwStopDuration = 1f;
        [SerializeField] private Animator anim;

        [Inject] private PlayerMovement _playerTarget;
        
        private MonkeyState _state;
        private float _throwCooldownLeft;
        private float _throwStopTimeLeft;
        private float _lostSightTime;
        private float _searchTime;
        private Vector2 _lastSeenPlayerPoint;
        private Vector2 _retreatPoint;
        private Vector2 _playerPoint;
        private bool _haveRetreatPoint;
        private bool _animThrow;

        public event Action<Vector2> OnBananaThrowRequested;

        private void Start()
        {
            ObjectInjector.InjectObject(this);
        }

        protected override void Tick()
        {
            anim.SetFloat(_moveX, aiPath.velocity.x);
            anim.SetFloat(_moveY, aiPath.velocity.y);
            anim.SetBool(_isMoving, aiPath.velocity.magnitude > 0);
            
            TickThrowCooldown();

            if (TickThrowStop())
                return;

            if (TrySeePlayer(out var playerPoint))
            {
                _lostSightTime = 0;
                _searchTime = 0;
                _lastSeenPlayerPoint = playerPoint;
                _state = MonkeyState.HOLD_DISTANCE;
                TickCombat(playerPoint);
                return;
            }

            _lostSightTime += Time.deltaTime;

            if (_state == MonkeyState.HOLD_DISTANCE)
            {
                if (_haveRetreatPoint && !IsPointReached(_retreatPoint, retreatPointReachDistance))
                {
                    MoveToPoint(_retreatPoint, retreatSpeedMultiplier);

                    if (!IsPathEndedFor(_retreatPoint, retreatPointReachDistance))
                        return;

                    _haveRetreatPoint = false;
                }

                if (_lostSightTime < lostSightDelay)
                {
                    TickCombat(_lastSeenPlayerPoint, false);
                    return;
                }

                _haveRetreatPoint = false;
                _searchTime = 0;
                _state = MonkeyState.SEARCH_LAST_SEEN;
            }

            if (_state == MonkeyState.SEARCH_LAST_SEEN)
            {
                TickSearchLastSeen();
                return;
            }

            _haveRetreatPoint = false;
            _searchTime = 0;
            _state = MonkeyState.PATROL;
            Patrol();
        }

        private void TickCombat(Vector2 playerPoint, bool canThrow = true)
        {
            var distanceToPlayer = Vector2.Distance(transform.position, playerPoint);

            if (distanceToPlayer <= retreatDistance)
            {
                MoveRetreat(playerPoint, retreatSpeedMultiplier);
                return;
            }

            if (distanceToPlayer < keepDistance)
            {
                MoveRetreat(playerPoint);

                if (canThrow)
                    TryThrowBanana(playerPoint);

                return;
            }

            _haveRetreatPoint = false;
            StopMoving();

            if (canThrow)
                TryThrowBanana(playerPoint);
        }

        private void TickSearchLastSeen()
        {
            _searchTime += Time.deltaTime;

            if (IsPointReached(_lastSeenPlayerPoint, searchPointReachDistance) || IsSearchPathEnded() ||
                _searchTime >= searchTimeout)
            {
                _state = MonkeyState.PATROL;
                _searchTime = 0;
                Patrol();
                return;
            }

            MoveToPoint(_lastSeenPlayerPoint);
        }

        private bool IsSearchPathEnded()
        {
            return IsPathEndedFor(_lastSeenPlayerPoint, searchPointReachDistance);
        }

        private void MoveRetreat(Vector2 playerPoint, float speedMultiplier = 1f)
        {
            if (!_haveRetreatPoint || IsPointReached(_retreatPoint, retreatPointReachDistance) ||
                Vector2.Distance(playerPoint, _retreatPoint) < keepDistance ||
                IsPathEndedFor(_retreatPoint, retreatPointReachDistance))
            {
                _retreatPoint = GetSafePoint(playerPoint);
                _haveRetreatPoint = true;
            }

            MoveToPoint(_retreatPoint, speedMultiplier);
        }

        private bool IsPathEndedFor(Vector2 point, float reachDistance)
        {
            if (!aiPath || aiPath.pathPending)
                return false;

            if (!aiPath.hasPath)
                return true;

            return aiPath.reachedEndOfPath && !IsPointReached(point, reachDistance);
        }

        private Vector2 GetSafePoint(Vector2 playerPoint)
        {
            var directionFromPlayer = ((Vector2)transform.position - playerPoint).normalized;

            if (directionFromPlayer.sqrMagnitude <= Mathf.Epsilon)
                directionFromPlayer = Random.insideUnitCircle.normalized;

            return playerPoint + directionFromPlayer * safeDistance;
        }

        private bool TrySeePlayer(out Vector2 playerPoint)
        {
            playerPoint = default;

            if (!_playerTarget)
                return false;

            playerPoint = _playerTarget.transform.position;
            var direction = playerPoint - (Vector2)transform.position;
            var distance = direction.magnitude;

            if (distance > viewDistance)
                return false;

            if (distance <= Mathf.Epsilon)
                return true;

            var hit = Physics2D.Raycast(
                transform.position,
                direction.normalized,
                distance,
                playerLayers | sightBlockLayers);

            return hit.collider && LayerService.CheckLayersEquality(hit.collider.gameObject.layer, playerLayers);
        }

        private void TryThrowBanana(Vector2 playerPoint)
        {
            if (_throwCooldownLeft > 0)
                return;

            _throwCooldownLeft = throwCooldown;
            _throwStopTimeLeft = throwStopDuration;
            var direction = (playerPoint - (Vector2)transform.position).normalized;
            
            anim.SetFloat(_throwX, direction.x);
            anim.SetFloat(_throwY, direction.y);
            anim.SetTrigger(_throw);
            
            StopMoving();
            _playerPoint = playerPoint;
        }

        private void TickThrowCooldown()
        {
            if (_throwCooldownLeft <= 0)
                return;

            _animThrow = false;
            _throwCooldownLeft -= Time.deltaTime;
        }

        private bool TickThrowStop()
        {
            if (_throwStopTimeLeft <= 0)
                return false;

            _throwStopTimeLeft -= Time.deltaTime;
            StopMoving();
            return true;
        }

        public void TriggerThrow()
        {
            if(_animThrow)
                return;
            
            OnBananaThrowRequested?.Invoke(_playerPoint);
            _animThrow = true;
        }

        private enum MonkeyState : byte
        {
            PATROL = 0,
            HOLD_DISTANCE = 1,
            SEARCH_LAST_SEEN = 2
        }
    }
}
