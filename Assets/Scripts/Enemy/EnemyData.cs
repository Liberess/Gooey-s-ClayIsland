using UnityEngine;

namespace Hun.Entity
{
    [CreateAssetMenu(fileName = "Enemy Data", menuName = "Scriptable Object/Enemy Data", order = int.MaxValue)]
    [System.Serializable]
    public class EnemyData : ScriptableObject
    {
        [SerializeField] private string myName;
        public string MyName { get => myName; }

        [SerializeField] private int hp;
        public int Hp { get => hp; }

        [Header("Target Setting")]
        [SerializeField] private LayerMask whatIsTarget;
        public LayerMask WhatIsTarget { get => whatIsTarget; }

        [Header("View Setting"), Space(10)]
        [SerializeField] private float fieldOfView;
        public float FieldOfView { get => fieldOfView; }

        [SerializeField] private float viewDistance;
        public float ViewDistance { get => viewDistance; }

        [Header("Move Setting"), Space(10)]
        [SerializeField] private float runSpeed = 10f;
        public float RunSpeed { get => runSpeed; }

        [SerializeField] private float patrolSpeed;
        public float PatrolSpeed { get => patrolSpeed; }

        [SerializeField, Range(0.01f, 2f)] private float turnSmoothTime = 0.1f; // 방향 회전 지연 시간
        public float TurnSmoothTime { get => turnSmoothTime; }

        [Header("Attack Setting"), Space(10)]
        [SerializeField] private int damage;
        public int Damage { get => damage; }

        [SerializeField] private float attackRadius = 2f;
        public float AttackRadius { get => attackRadius; }

        public void Setup(int _health, int _damage, float _runSpeed, float _patrolSpeed)
        {
            hp = _health;
            damage = _damage;
            runSpeed = _runSpeed;
            patrolSpeed = _patrolSpeed;
        }
    }
}