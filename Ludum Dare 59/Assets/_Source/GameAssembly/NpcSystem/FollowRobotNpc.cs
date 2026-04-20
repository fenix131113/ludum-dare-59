using Player;
using UnityEngine;
using Utils;
using VContainer;

namespace NpcSystem
{
    public class FollowRobotNpc : BaseNpc
    {
        private static readonly int _moveX = Animator.StringToHash("MoveX");
        private static readonly int _moveY = Animator.StringToHash("MoveY");
        private static readonly int _isMoving = Animator.StringToHash("IsMoving");

        [SerializeField] private LayerMask playerLayers;
        [SerializeField] private LayerMask sightBlockLayers;
        [SerializeField] private float viewDistance;
        [SerializeField] private float searchPointReachDistance;
        [SerializeField] private float searchTimeout = 2f;
        [SerializeField] private float chaseSpeedMultiplier = 1.15f;
        [SerializeField] private float stopDuration = 1f;
        [SerializeField] private Collider2D triggerCollider;
        [SerializeField] private Animator anim;

        [Inject] private PlayerMovement _playerTarget;

        private FollowRobotState _state;
        private float _searchTime;
        private float _stopTimeLeft;
        private Vector2 _lastSeenPlayerPoint;
        private bool _isTriggerDisabled;

        private void Start()
        {
            ObjectInjector.InjectObject(this);
        }

        protected override void Tick()
        {
            anim.SetFloat(_moveX, aiPath.velocity.x);
            anim.SetFloat(_moveY, aiPath.velocity.y);
            anim.SetBool(_isMoving, aiPath.velocity.magnitude > 0);

            if (TickStop())
                return;

            if (TrySeePlayer(out var playerPoint))
            {
                _searchTime = 0;
                _lastSeenPlayerPoint = playerPoint;
                _state = FollowRobotState.CHASE;
                MoveToPoint(playerPoint, chaseSpeedMultiplier);
                return;
            }

            if (_state == FollowRobotState.CHASE)
            {
                _searchTime = 0;
                _state = FollowRobotState.SEARCH_LAST_SEEN;
            }

            if (_state == FollowRobotState.SEARCH_LAST_SEEN)
            {
                TickSearchLastSeen();
                return;
            }

            _searchTime = 0;
            _state = FollowRobotState.PATROL;
            Patrol();
        }

        private void TickSearchLastSeen()
        {
            _searchTime += Time.deltaTime;

            if (IsPointReached(_lastSeenPlayerPoint, searchPointReachDistance) || IsSearchPathEnded() ||
                _searchTime >= searchTimeout)
            {
                _state = FollowRobotState.PATROL;
                _searchTime = 0;
                Patrol();
                return;
            }

            MoveToPoint(_lastSeenPlayerPoint);
        }

        private bool IsSearchPathEnded()
        {
            if (!aiPath || aiPath.pathPending)
                return false;

            if (!aiPath.hasPath)
                return true;

            return aiPath.reachedEndOfPath && !IsPointReached(_lastSeenPlayerPoint, searchPointReachDistance);
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

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!enabled || !LayerService.CheckLayersEquality(other.gameObject.layer, playerLayers))
                return;

            TriggerStop();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!enabled || !LayerService.CheckLayersEquality(other.gameObject.layer, playerLayers))
                return;

            TriggerStop();
        }

        private void TriggerStop()
        {
            if (_stopTimeLeft > 0)
                return;

            _stopTimeLeft = stopDuration;
            StopMoving();

            if (!triggerCollider || !triggerCollider.enabled)
                return;

            triggerCollider.enabled = false;
            _isTriggerDisabled = true;
        }

        private bool TickStop()
        {
            if (_stopTimeLeft <= 0)
                return false;

            _stopTimeLeft -= Time.deltaTime;
            StopMoving();

            if (_stopTimeLeft > 0)
                return true;

            if (triggerCollider && _isTriggerDisabled)
                triggerCollider.enabled = true;

            _isTriggerDisabled = false;
            return false;
        }

        private enum FollowRobotState : byte
        {
            PATROL = 0,
            CHASE = 1,
            SEARCH_LAST_SEEN = 2
        }
    }
}