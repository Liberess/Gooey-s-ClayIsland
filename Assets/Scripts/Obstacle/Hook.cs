using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Obstacle
{
    public class Hook : MonoBehaviour
    {
        [SerializeField, Range(0f, 50f)] private float hookForce = 25f;

        private Hun.Player.PlayerGrappler grappler;
        private Transform shootTransform;
        private Transform targetTransform;

        private Rigidbody rigid;
        private LineRenderer lineRen;

        private Vector3[] positions = new Vector3[2];

        private void Awake()
        {
            grappler = null;
            targetTransform = null;
            shootTransform = null;
        }

        private void Update()
        {
            if (shootTransform == null)
                return;

            positions[0] = transform.position;
            positions[1] = shootTransform.position;

            lineRen.SetPositions(positions);
        }

        public void InitializeHook(Hun.Player.PlayerGrappler _grappler, Transform hookPos, Transform _shootTransform)
        {
            grappler = _grappler;
            transform.forward = _shootTransform.forward;
            shootTransform = _shootTransform;
            targetTransform = hookPos;

            rigid = GetComponent<Rigidbody>();
            lineRen = GetComponent<LineRenderer>();

            transform.LookAt(targetTransform);

            StartCoroutine(HookCo());
            
            //rigid.AddForce(transform.forward * hookForce, ForceMode.Impulse);
        }

        private IEnumerator HookCo()
        {
            while(true)
            {
                var gap = Vector3.Distance(transform.position, targetTransform.position);

                if (gap <= 0.1f)
                    break;

                transform.position = Vector3.MoveTowards(transform.position,
                    targetTransform.position, hookForce * Time.deltaTime);

                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if((LayerMask.GetMask("Grapple") & 1 << other.gameObject.layer) > 0)
            {
                rigid.useGravity = false;
                rigid.isKinematic = true;
                grappler.StartPull();
            }
        }
    }
}