using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Obstacle
{
    public class NeedleSwitch : MonoBehaviour, IObstacle
    {
        private enum SwitchType { Auto, Enter }

        [SerializeField] private SwitchType switchType = SwitchType.Auto;

        [SerializeField, Range(0.0f, 5.0f)] private float hitRange = 1.5f;
        [SerializeField, Range(0f, 10f)] private float activeDelayTime = 3f;
        [SerializeField, Range(0f, 10f)] private float inactiveDelayTime = 1f;
        
        private bool isActive = false;

        private WaitForSeconds activeDelay;
        private WaitForSeconds inactiveDelay;

        private Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        private void Start()
        {
            activeDelay = new WaitForSeconds(activeDelayTime);
            inactiveDelay = new WaitForSeconds(inactiveDelayTime);

            if (switchType == SwitchType.Auto)
                StartCoroutine(ActiveCo());
        }

        public void OnEnter()
        {
            if (switchType == SwitchType.Auto)
                return;

            if (!isActive)
                StartCoroutine(ActiveCo());
        }

        private IEnumerator ActiveCo()
        {
            if(!isActive)
            {
                isActive = true;
                yield return activeDelay;
                anim.ResetTrigger("doActive");
                anim.SetTrigger("doActive");
            }
            else
            {
                yield return inactiveDelay;
                isActive = false;

                if (switchType == SwitchType.Auto)
                    StartCoroutine(ActiveCo());
            }
        }

        public void OnExit()
        {

        }

        public void OnInteract()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position + new Vector3(0f, 2f, -0.4f), hitRange, LayerMask.GetMask("Player"));

            if (hitColliders != null && hitColliders.Length > 0)
            {
                foreach(var hitCollider in hitColliders)
                {
                    if (hitCollider.TryGetComponent<Entity.LivingEntity>(out Entity.LivingEntity player))
                    {
                        Entity.DamageMessage dmgMsg = new Entity.DamageMessage();
                        dmgMsg.damager = gameObject;
                        dmgMsg.dmgAmount = 1;
                        dmgMsg.hitNormal = transform.position;
                        dmgMsg.hitPoint = transform.position;
                        player.ApplyDamage(dmgMsg);
                    }
                }
            }
        }
    }
}