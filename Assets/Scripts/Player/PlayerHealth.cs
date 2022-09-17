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
            OnDeathEvent += RespawnCheckPoint;
            OnGameOverEvent += LoadWorldMap;
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

        public override void RestoreLife(int value)
        {
            base.RestoreLife(value);
        }

        private void RespawnCheckPoint()
        {
            GameObject.FindGameObjectWithTag("Player").
                GetComponent<PlayerController>().TeleportToCheckPoint();
        }

        private void LoadWorldMap()
        {
            Manager.GameManager.Instance.LoadScene("WorldMapScene");
        }
    }
}