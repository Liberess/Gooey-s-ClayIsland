using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Entity.Player
{
    public class PlayerHealth : LivingEntity
    {
        private void Start()
        {
            OnSpawned();
            //Manager.UIManager.Instance.SetHeartUI(Heart);
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
    }
}