using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

namespace GGJ2022.EnemyAI
{
    public class EnemyState : MonoBehaviour
    {
        public enum States
        {
            Idle,
            DoingMelee,
            DoingRanged,
            Patrolling,
            Seeking,
            Fleeing,
            Dead
        }

        [SerializeField] private float _maxAggro = 100f;

        [SerializeField] private float _aggroCoolDownRate = 15f;

        [SerializeField] private float _aggroLevel = 0f;

        public float AggroLevel => _aggroLevel;

        [SerializeField] private float _sightRadius = 5f; // meters
        public float SightRadius => _sightRadius;

        [SerializeField] private float _attackRangeRadius = 3f;

        [SerializeField] private bool _canFly = false;
        public bool CanFly => _canFly;

        [SerializeField] private bool _shouldFlee = true;

        private States _state = States.Idle;
        public States State => _state;

        [SerializeField] private bool _shouldPatrol = true;

        private bool _isPlayerInRange = false;

        private bool _isPlayerInSight = false;
        public bool IsPlayerInSight => _isPlayerInSight;

        private Vector3 _destinationPosition;

        [SerializeField] private Transform _attackTarget;

        private bool _isPlayerObstructed = false;

        [SerializeField] private bool _isRangedAttackEnabled = true;
        [SerializeField] private bool _isMeleeAttackEnabled = true;

        public bool HasRanged => _isRangedAttackEnabled;
        public bool HasMelee => _isMeleeAttackEnabled;

        [SerializeField] private float _maxWalkSpeed = 3f;
        [SerializeField] private float _maxRunSpeed = 5f;

        [SerializeField] private float _healthFleeingThreshold = 1f;

        [SerializeField] private int _maxHealth = 5;
        public int MaxHealth => _maxHealth;

        private int _health;
        public int Health => _health;

        private bool _isAttackInProgress = false;

        [SerializeField] private float _deathTimeout = 2f; // seconds til destroy 
        private float _timeOfDeath;

        [SerializeField] private LayerMask _layersToExclude;
        private int _exclusionMask;

        private bool _isMoving = false;
        public bool IsMoving => _isMoving;

        private bool _canMove = true;

        private bool _isLineOfSightObstructed = false; 

        // components
        private Collider _collider;
        private Rigidbody _rigidbody;



        // TODO: scriptable object that contains animation, weapon object references
        private ScriptableObject _data;

        public delegate void Died();

        public delegate void DeathTimedOut();

        public delegate void StartedMoving();

        public delegate void StoppedMoving();

        public delegate void SawPlayer();

        public delegate void LostSightOfPlayer();

        public delegate void StartedFleeing();

        public delegate void ReachedPlayer();

        public delegate void StartedAttacking(States state);

        public delegate void StoppedAttacking();

        public delegate void Damaged();

        public Died OnDied;
        public Damaged OnDamaged;
        public StartedMoving OnStartedMoving;
        public StoppedMoving OnStoppedMoving;
        public SawPlayer OnSawPlayer;
        public LostSightOfPlayer OnLostSightOfPlayer;
        public StartedFleeing OnStartedFleeing;
        public ReachedPlayer OnReachedPlayer;
        public StartedAttacking OnStartedAttacking;
        public StoppedAttacking OnStoppedAttacking;
        public DeathTimedOut OnDeathTimedOut;

        void ProcessState()
        {
            Vector3? attempt;
            switch (_state)
            {
                case States.Dead:

                    _canMove = false;

                    if (Time.time - _timeOfDeath > _deathTimeout)
                    {
                        OnDeathTimedOut?.Invoke();
                        Destroy(gameObject);
                    }

                    break;

                case States.Fleeing:
                    bool flag = CheckIsPlayerInSightRange();

                    if (flag && (transform.position == _destinationPosition || !_isMoving))
                    {
                        attempt = ChooseFleeingTarget();

                        if (attempt == null)
                        {
                            TriggerStateChange(States.Idle);
                            break;
                        }

                        _destinationPosition = (Vector3) attempt;
                    }
                    else if (!flag)
                    {
                        TriggerStateChange(States.Idle);
                    }

                    break;

                case States.Idle:

                    if (CheckIsPlayerInSightRange())
                    {
                        TriggerStateChange(States.Seeking);
                    }
                    else if (_shouldPatrol)
                    {
                        TriggerStateChange(States.Patrolling);
                    }

                    break;

                case States.Patrolling:
                    if (CheckIsPlayerInSightRange())
                    {
                        TriggerStateChange(States.Seeking);
                    }
                    else if (transform.position == _destinationPosition || !_isMoving)
                    {
                        attempt = ChoosePatrolPoint();

                        if (attempt == null)
                        {
                            TriggerStateChange(States.Idle);
                            break;
                        }

                        _destinationPosition = (Vector3) attempt;
                    }

                    break;

                case States.Seeking:
                    if (_shouldFlee && _health <= _healthFleeingThreshold)
                    {
                        TriggerStateChange(States.Fleeing);
                        break;
                    }

                    if (CheckIsPlayerInSightRange())
                    {

                        if (_isRangedAttackEnabled)
                        {
                            Vector3? result = ChooseRangedPosition();

                            if (result == null)
                            {
                                TriggerStateChange(_isMeleeAttackEnabled ? States.Seeking : States.Idle);
                            }
                            else if (!_isMoving)
                            {
                                _destinationPosition = (Vector3) result;
                            }
                        }
                        else if (CheckIsSightlineObstructed())
                        {
                            attempt = ChooseNavigationTarget();

                            if (attempt == null)
                            {
                                TriggerStateChange(States.Idle);
                                break;
                            }

                            _destinationPosition = (Vector3) attempt;
                        }
                        else
                        {
                            _destinationPosition = _attackTarget.position;
                        }
                    }
                    else
                    {
                        TriggerStateChange(States.Idle);
                    }

                    break;

                case States.DoingMelee:
                case States.DoingRanged:
                    if (_isAttackInProgress)
                    {
                        return;
                    }

                    if (_state == States.DoingRanged)
                    {
                        if (CheckIsPlayerInRangedAttackRadius())
                        {
                            _canMove = false;
                            _isAttackInProgress = true;
                            OnStartedAttacking?.Invoke(_state);
                        }
                        else
                        {
                            _canMove = true;
                            _isAttackInProgress = false;
                            TriggerStateChange(States.Seeking);
                        }
                    }
                    else if (_state == States.DoingMelee && !_isMoving
                                                         && GetDistanceToPlayer() < 0.4f)
                    {
                        _canMove = false;
                        _isAttackInProgress = true;
                        OnStartedAttacking?.Invoke(_state);
                    }
                    else
                    {
                        _canMove = true;
                    }

                    break;
            }
        }


        bool GetIsPositionOccupied(Vector3 position)
        {
            return Physics.OverlapBox(position + _collider.bounds.center, _collider.bounds.extents, Quaternion.identity,
                    _exclusionMask)
                .Length < 1;
        }

        Vector3 NudgeTarget(Vector3 position)
        {
            float rotationInterval = 30f; // degrees
            float intervals = Random.Range(0f, 360f / rotationInterval - 1f);

            float rotationAmount = intervals * rotationInterval;

            Quaternion rotation = Quaternion.Euler(0, rotationAmount, 0);

            float nudgeDistance = 1f; // meters
            Vector3 relativeNudge = nudgeDistance * (rotation * Vector3.forward);

            return position + relativeNudge;
        }

        Vector3? GetValidTarget(Vector3 position)
        {
            // max 12 nduges
            for (int i = 0; i < 12; i++)
            {
                if (GetIsPositionOccupied(position))
                {
                    position = NudgeTarget(position);
                }
                else
                {
                    return position;
                }
            }

            return null;
        }

        Vector3? ChooseRangedPosition()
        {
            // find a place to stand from where we can fire
            // assumes that we're already in range

            return GetValidTarget(transform.position);
        }

        Vector3? ChooseNavigationTarget()
        {
            // pick a point to circumvent player obstruction 

            float rayDistance = 0.5f * GetDistanceToPlayer();

            float sweepAngle = 120f;
            float sweepOffset = 90f - 0.5f * sweepAngle;

            Vector3 rightVector = Vector3.Cross(Vector3.up, _attackTarget.position - transform.position).normalized;

            for (int i = 0; i < 10; i++)
            {
                float eulerDirection = sweepOffset + Random.value * sweepAngle;
                Vector3 castVector = Quaternion.Euler(0, eulerDirection, 0) * (rayDistance * rightVector);

                if (!Physics.Raycast(_collider.bounds.center, castVector, rayDistance))
                {
                    return transform.position + castVector;
                }
            }

            return null;
        }

        Vector3? ChooseFleeingTarget()
        {
            // todo: running away from player
            return GetValidTarget(transform.position + 0.5f * (transform.position - _attackTarget.position));
        }

        bool CheckIsPlayerInSightRange()
        {
            _isPlayerInSight = GetDistanceToPlayer() < _sightRadius;
            return _isPlayerInSight;
        }

        bool CheckIsSightlineObstructed()
        {
            Vector3 castVector = _attackTarget.position - transform.position;

            RaycastHit hit;
            if (Physics.Raycast(_collider.bounds.center, castVector, out hit, castVector.magnitude))
            {
                _isLineOfSightObstructed = hit.collider.transform != _attackTarget;
            }

            _isLineOfSightObstructed = false; 
            return _isLineOfSightObstructed;
        }

        Vector3? ChoosePatrolPoint()
        {
            float range = 360f;
            var rotation = Quaternion.Euler(0, Random.Range(0f, range), 0);

            float dist = Random.Range(1f, 4f); // meters

            var vector = rotation * (dist * transform.forward);

            return GetValidTarget(transform.position + vector);
        }

        void TriggerStateChange(States state)
        {
            if (_state == state)
            {
                return;
            }

            if ((_state is (States.DoingMelee or States.DoingRanged)) &&
                (state is not (States.DoingMelee or States.DoingRanged)))
            {
                _isAttackInProgress = false;
                OnStoppedAttacking?.Invoke();
            }

            _state = state;

            Vector3? attempt;

            switch (state)
            {
                case States.Dead:
                    _timeOfDeath = Time.time;
                    OnDied?.Invoke();
                    break;

                case States.Fleeing:

                    attempt = ChooseFleeingTarget();

                    if (attempt == null)
                    {
                        TriggerStateChange(States.Idle);
                        break;
                    }

                    _destinationPosition = (Vector3) attempt;
                    OnStartedFleeing?.Invoke();
                    break;

                case States.Idle:
                    _destinationPosition = transform.position; 
                    break;

                case States.Patrolling:
                    attempt = ChoosePatrolPoint();

                    if (attempt == null)
                    {
                        TriggerStateChange(States.Idle);
                        break;
                    }

                    _destinationPosition = (Vector3) attempt;
                    break;

                case States.Seeking:
                    break;

                case States.DoingMelee:
                case States.DoingRanged:
                    if (_isAttackInProgress)
                    {
                        break;
                    }

                    _isAttackInProgress = true;
                    OnStartedAttacking(_state);
                    break;
            }
        }

        void HandleOnSawPlayer()
        {
            TriggerStateChange(States.Seeking);
        }

        bool CheckIsPlayerInRangedAttackRadius()
        {
            if (!_isRangedAttackEnabled)
            {
                return false;
            }

            if (GetDistanceToPlayer() < _attackRangeRadius)
            {
                return true;
            }

            return false;
        }

        float GetDistanceToPlayer()
        {
            return (_attackTarget.position - transform.position).magnitude;
        }

        void HandleOnReachedPlayer()
        {
            TriggerStateChange(States.DoingMelee);
        }

        void ProcessStatsUpdates()
        {
            _aggroLevel -= _aggroCoolDownRate * Time.deltaTime;
            _aggroLevel = Mathf.Clamp(_aggroLevel, 0f, _maxAggro);
        }

        // Use this for initialization
        void Start()
        {
            _health = _maxHealth;
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
            _exclusionMask = ~_layersToExclude;

            _destinationPosition = transform.position; 

        }

        // Update is called once per frame
        void Update()
        {
            ProcessStatsUpdates();
            
            ProcessState();
        }

        private void FixedUpdate()
        {
            var directionVector = _destinationPosition - transform.position;
            float distanceToDestination = directionVector.magnitude;
            if (_canMove && distanceToDestination > 0.4f)
            {
                _isMoving = true;

                // take out vertical component 
                var moveDirection = (directionVector - Vector3.Dot(Vector3.up, directionVector) * Vector3.up)
                    .normalized;

                float speed = _maxWalkSpeed;

                if (_state == States.Fleeing || _aggroLevel > 0.35f * _maxAggro)
                {
                    speed = _maxRunSpeed;
                }

                float acceleration = 3f * speed;

                if (_rigidbody.velocity.magnitude < speed)
                {
                    _rigidbody.velocity += acceleration * Time.fixedDeltaTime * moveDirection;
                }
            }
            else
            {
                // apply lateral friction 
                var vertical = Vector3.Dot(Vector3.up, _rigidbody.velocity) * Vector3.up;
                var lateral = _rigidbody.velocity - vertical;

                _rigidbody.velocity = 0.85f * lateral + vertical;

                if (_rigidbody.velocity.magnitude < 0.05f)
                {
                    _isMoving = false; 
                }
            }
        }

        public void DoDamage(int damage)
        {
            _health = Math.Max(0, _health - damage);

            if (_health <= 0)
            {
                TriggerStateChange(States.Dead);
            }

            _aggroLevel += 4f * damage; 
        }
    }
}
