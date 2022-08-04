using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hun.Player;

namespace Hun.Obstacle
{
    public abstract class Grapple : MonoBehaviour
    {
        protected PlayerGrappler grappler;
        protected PlayerGrappler.GrapState grapState;

        //public bool IsActive { get; protected set; }
        public bool IsActive;
        //public float PullTimer { get; protected set; }
        public float PullTimer;
        protected float pullMaxTime = 2.0f;

        private Animator anim;

        protected abstract void Initialize();
        protected abstract void Complete();

        private void Awake()
        {
            grappler = FindObjectOfType<PlayerGrappler>();
            anim = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            Initialize();
        }

        public virtual void Progress()
        {
            if (!IsActive)
                return;

            if (PullTimer >= pullMaxTime)
            {
                IsActive = false;
                Complete();
            }
            else
            {
                PullTimer += Time.deltaTime;
            }
        }

        protected virtual void OnDown()
        {
            grappler.SetGrapState(grapState);
        }

        public void SetAnimTrigger(string triggerName) => anim.SetTrigger(triggerName);

        private void OnMouseEnter()
        {
            OnDown();
        }

        private void OnMouseDown()
        {
            OnDown();
        }
    }
}