using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Obstacle
{
    public class GrapPull : Grapple
    {
        protected override void Initialize()
        {
            IsActive = true;
            PullTimer = 0.0f;
            grapState = Player.PlayerGrappler.GrapState.Pull;
        }

        protected override void Complete()
        {
            var effect = transform.Find("Particle System").GetComponent<ParticleSystem>();
            effect.gameObject.SetActive(true);
            effect.Play();
        }

        public void Open() => StartCoroutine(OpenCo());

        private IEnumerator OpenCo()
        {
            var targetPos =transform.forward * 2f;

            while (true)
            {
                targetPos.y = transform.position.y;

                transform.position = Vector3.MoveTowards(
                    transform.position, targetPos, Time.deltaTime);

                yield return null;
            }
        }
    }
}