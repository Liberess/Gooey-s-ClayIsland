using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Obstacle
{
    public class Spike : MonoBehaviour, IObstacle
    {
        public void OnEnter()
        {
            var player = FindObjectOfType<Entity.LivingEntity>();
            if(player != null)
            {
                Entity.DamageMessage dmgMsg = new Entity.DamageMessage();
                dmgMsg.damager = gameObject;
                dmgMsg.dmgAmount = 1;
                dmgMsg.hitNormal = transform.position;
                dmgMsg.hitPoint = transform.position;
                player.ApplyDamage(dmgMsg);
            }
        }

        public void OnExit()
        {

        }

        public void OnInteract()
        {

        }
    }
}