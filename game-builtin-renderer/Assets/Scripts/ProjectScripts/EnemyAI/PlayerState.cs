using UnityEngine;
using System;
using Unity.VisualScripting;

namespace GGJ2022.EnemyAI
{
    [RequireComponent(typeof(CharacterAnimationManager))]
    [RequireComponent(typeof(RelativeCharacterController))]
    public class PlayerState : MonoBehaviour
    {
        [SerializeField] private GameObject _projectile;
        [SerializeField] private int _health;
        [SerializeField] private int _maxHealth = 20;

        public int Health => _health;
        public int MaxHealth => _maxHealth;

        private bool _isDead;
        public bool IsDead => _isDead; 
        
        public delegate void Died();

        public Died OnDied;

        [SerializeField] private int _meleeDamage = 1;
        public int MeleeDamage => _meleeDamage; 
        
        public enum States
        {
            Idle,
            DoingMelee,
            DoingRanged,
            DoingAOE,
            Walking,
            Dead
        }

        private States _state = States.Idle;

        // components
        private CharacterAnimationManager _animationManager;
        private RelativeCharacterController _controller;

        [SerializeField] private Collider _boundsCollider; 

        private void Awake()
        {
            _animationManager = GetComponent<CharacterAnimationManager>();
            _controller = GetComponent<RelativeCharacterController>();
        }

        private void OnEnable()
        {
            _animationManager.OnActionStarted += HandleInputAction;
        }

        private void OnDisable()
        {
            throw new NotImplementedException();
        }

        void HandleInputAction(string action)
        {
            if (_isDead) return;
            switch (action)
            {
                case "Melee":
                    DoMelee();
                    break;

                case "Ranged":
                    DoRanged();
                    break;
            }
        }

        void DoMelee()
        {
            var direction = _controller.GetIntendedSpatialDirection(); 
            var attackCenter = 0.4f * direction + _boundsCollider.bounds.center;

            var colliders = Physics.OverlapBox(attackCenter, 1.13f * _boundsCollider.bounds.extents);

            foreach (var c in colliders)
            {
                var state = c.gameObject.GetComponent<EnemyState>();

                if (state == null) continue; 
                
                state.DoDamage(_meleeDamage);
                c.attachedRigidbody.AddForce(400f * (c.bounds.center - _boundsCollider.bounds.center).normalized);
            }
        }

        void DoRanged()
        {
            var direction = _controller.GetIntendedSpatialDirection(); 
            var attackCenter = 0.4f * direction + _boundsCollider.bounds.center;
            var p = Instantiate(_projectile).GetComponent<ProjectileBehaviour>();
            p.transform.position = attackCenter;

            p.parent = gameObject;
            p.velocity = 20f * GetSeekDirection(direction); 
        }

        Vector3 GetSeekDirection(Vector3 baseDirection)
        {
            float seekRadius = 1.5f; // meters

            var colliders = Physics.SphereCastAll(_boundsCollider.bounds.center, seekRadius, baseDirection, 7f);

            foreach (var c in colliders)
            {
                if (c.transform.GetComponent<EnemyState>())
                {
                    return (c.transform.position - _boundsCollider.bounds.center).normalized; 
                }
            }
            
            return baseDirection; 
        }

        public void DoDamage(int damage)
        {
            _health = Math.Max(0, _health - damage);

            if (_health <= 0)
            {
                _controller.SetIsInputEnabled(false);
                _isDead = true; 
                OnDied?.Invoke();
            }
        }
    }
}