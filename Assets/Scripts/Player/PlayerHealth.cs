using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hun.Entity;

namespace Hun.Player
{
    public class PlayerHealth : LivingEntity
    {
        private void Start()
        {
            OnSpawned();
            OnDeathEvent += Manager.GameManager.Instance.PlayerDie;
            OnDeathEvent += Manager.GameManager.Instance.PlayerDie;
            OnGameOverEvent += Manager.GameManager.Instance.GameOver;
        }

        private void OnEnable()
        {

        }

        public override void ApplyDamage(DamageMessage dmgMsg)
        {
            base.ApplyDamage(dmgMsg);
            //Manager.UIManager.Instance.SetHeartUI(Heart);
        }

        public override void RestoreHeart(int value)
        {
            base.RestoreHeart(value);
            //Manager.UIManager.Instance.SetHeartUI(Heart);
        }

        public override void RestoreLife(int value)
        {
            base.RestoreLife(value);
        }
    }
}