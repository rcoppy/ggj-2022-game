using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

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

        private States _state;
        public States State = States.Idle;

        [SerializeField] private bool _shouldPatrol = true;

        private bool _isPlayerInRange = false; 
        
        private bool _isPlayerInSight = false;
        public bool IsPlayerInSight => _isPlayerInSight;

        private Vector3 _destinationPosition;

        private Transform _attackTarget; 

        private bool _isPlayerObstructed = false;

        [SerializeField] private bool _isRangedAttackEnabled = true;
        [SerializeField] private bool _isMeleeAttackEnabled = true;

        public bool HasRanged => _isRangedAttackEnabled;
        public bool HasMelee => _isMeleeAttackEnabled;

        [SerializeField] private float _maxWalkSpeed = 3f;
        [SerializeField] private float _maxRunSpeed = 5f;

        [SerializeField] private float _healthFleeingThreshold = 1f; 
        
        [SerializeField] private float _maxHealth = 5f;
        public float MaxHealth => _maxHealth;

        private float _health;
        public float Health => _health;

        private bool _isAttackInProgress = false;

        [SerializeField] private float _deathTimeout = 2f; // seconds til destroy 
        private float _timeOfDeath; 
        
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
            switch (_state)
            {
                case States.Dead:
                    if (Time.time - _timeOfDeath > _deathTimeout)
                    {
                        OnDeathTimedOut?.Invoke();
                        Destroy(gameObject);
                    }
                    break;
                
                case States.Fleeing:
                    if (CheckIsPlayerInSightRange() && transform.position == _destinationPosition)
                    {
                        // todo: make functional not oop 
                        ChooseFleeingTarget();
                    }
                    else
                    {
                        TriggerStateChange(States.Idle);
                    }
                    break;
                
                case States.Idle:

                    if (CheckIsPlayerInSightRange())
                    {
                        TriggerStateChange(States.Seeking);
                    } else if (_shouldPatrol)
                    {
                        TriggerStateChange(States.Patrolling);
                    }
                    
                    break;
                
                case States.Patrolling:

                    if (transform.position == _destinationPosition)
                    {
                        _destinationPosition = ChoosePatrolPoint();
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
                            Vector3? result = ChooseRangedTarget();

                            if (result == null)
                            {
                                TriggerStateChange(States.Idle);
                            }
                            else
                            {
                                _destinationPosition = (Vector3) result; 
                            }
                        }
                        else if (CheckIsSightlineObstructed())
                        {
                            // todo: make style functional not oop 
                            ChooseNavigationTarget();
                        }
                        else
                        {
                            _destinationPosition = _attackTarget.position; 
                        }
                    }
                    else if (_shouldPatrol)
                    {
                        TriggerStateChange(States.Patrolling);
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
                            _isAttackInProgress = true; 
                            OnStartedAttacking?.Invoke(_state);
                        }
                        else
                        {
                            _isAttackInProgress = false; 
                            TriggerStateChange(States.Seeking);
                        }
                    } else if (_state == States.DoingMelee)
                    {
                        // todo: do a range check? 
                        _isAttackInProgress = true;
                        OnStartedAttacking?.Invoke(_state); 
                    }
                    
                    break;
            }
        }
        

        bool GetIsPositionOccupied(Vector3 position)
        {
            // todo
            return false; 
        }
        
        Vector3 NudgeTarget(Vector3 position)
        {
            // todo 
            return position; 
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

        Vector3? ChooseRangedTarget()
        {
            // todo: find a place to stand from where we can fire
            return null; 
        }
        
        void ChooseNavigationTarget()
        {
            // todo: pick a point to circumvent player obstruction 
        }
        
        void ChooseFleeingTarget()
        {
            // todo: running away from player
        }

        bool CheckIsPlayerInSightRange()
        {
            _isPlayerInSight = GetDistanceToPlayer() < _sightRadius;
            return _isPlayerInSight; 
        }

        bool CheckIsSightlineObstructed()
        {
            // todo
            return false; 
        }

        Vector3 ChoosePatrolPoint()
        {
            // todo: pick a point to walk to in range
            return transform.position; 
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

            switch (state)
            {
                case States.Dead:
                    break;
                case States.Fleeing:
                    ChooseFleeingTarget();
                    OnStartedFleeing?.Invoke();
                    break;
                case States.Idle:
                    break;
                case States.Patrolling:
                    break;
                case States.Seeking:
                    break;
                case States.DoingMelee:
                case States.DoingRanged:    
                    if (_isAttackInProgress)
                    {
                        return;
                    }
                    
                    // nothing for now
                    
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
            return 1f; 
        }

        void HandleOnReachedPlayer()
        {
            TriggerStateChange(States.DoingMelee);
        }

        void ProcessStatUpdates()
        {
            _aggroLevel -= _aggroCoolDownRate * Time.deltaTime;
            _aggroLevel = Mathf.Clamp(_aggroLevel, 0f, _maxAggro);
        }

        // Use this for initialization
        void Start()
        {
            _health = _maxHealth; 
            
        }

        // Update is called once per frame
        void Update()
        {
            ProcessStatUpdates();
        }
    }
}
