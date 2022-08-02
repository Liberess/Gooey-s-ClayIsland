using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Obstacle 
{
    public class BoardJudgmentObj : ClayBlock
    {
        [SerializeField] protected BoxCollider ownCollider;

        public override void OnEnter()
        {

        }

        public override void OnStay()
        {

        }

        public override void OnExit()
        {

        }

        public virtual void SetTriggerState(bool state)
        {
            ownCollider.isTrigger = state;
        }
    }
}
