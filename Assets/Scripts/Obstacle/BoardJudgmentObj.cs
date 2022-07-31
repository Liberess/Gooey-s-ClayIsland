using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Obstacle 
{
    public class BoardJudgmentObj : MonoBehaviour
    {
        [SerializeField] protected BoxCollider ownCollider;

        public virtual void SetTriggerState(bool state)
        {
            ownCollider.isTrigger = state;
        }
    }
}
