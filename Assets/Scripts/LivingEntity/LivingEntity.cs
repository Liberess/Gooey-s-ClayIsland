using System;
using UnityEngine;
using UnityEngine.Events;

namespace Hun.Entity
{
    public class LivingEntity : MonoBehaviour, IDamageable
    {
        [Header("== Basic Status ==")]
        public int originHeart = 3;
        [SerializeField] private int heart;
        public int Heart 
        {
            get => heart;
            protected set
            {
                heart = value;
                OnHeartChanged();
            }
        }

        public int maxLife = 3;
        [SerializeField] private int life;
        public int Life
        {
            get => life;
            protected set
            {
                life = value;
                OnLifeChanged();
            }
        }

        /// <summary>
        /// 생명 변경시 발생하는 메서드
        /// </summary>
        private void OnLifeChanged()
        {
            if (life < 0)
                GameOver();

            Manager.UIManager.Instance.SetLifeUI(life);
        }

        private void OnHeartChanged()
        {
            if(heart <= 0)
            {
                --Life;
                heart = originHeart;
                Die();
            }

            Manager.UIManager.Instance.SetHeartUI(heart);
        }

        public bool IsDead { get; protected set; }

        public event UnityAction OnDeathEvent;
        public event UnityAction OnGameOverEvent;

        private const float minTimeBetDamaged = 0.1f; // 공격 허용할 딜레이
        private float lastDamagedTime;

        protected bool IsInvulerable
        {
            get
            {
                if (Time.time >= lastDamagedTime + minTimeBetDamaged)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// LivingEntity가 Spawn되면 상태를 초기화 해주는 메서드이다.
        /// </summary>
        protected virtual void OnSpawned()
        {
            IsDead = false;
            Heart = originHeart;
            Life = maxLife;
        }

        public virtual void ApplyDamage(DamageMessage dmgMsg)
        {
            if (IsInvulerable || IsDead)
                return;

            lastDamagedTime = Time.time;
            Heart -= dmgMsg.dmgAmount;

/*            if (heart <= 0 && !IsDead)
            {
                heart = 0;
                Die();
            }*/
        }

        private void ApplyUpdate(int newHealth, bool newIsDead)
        {
            heart = newHealth;
            IsDead = newIsDead;
        }

        public virtual void RestoreHeart(int value)
        {
            if (IsDead)
                return;

            if (heart + value >= originHeart)
                heart = originHeart;
            else
                heart += value;
        }

        public virtual void Die()
        {
            OnDeathEvent?.Invoke();
        }

        public virtual void GameOver()
        {
            OnGameOverEvent?.Invoke();

            IsDead = true;
        }
    }
}