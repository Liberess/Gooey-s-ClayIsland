using UnityEngine;
using Hun.Player;

namespace Hun.Obstacle
{
    public class Canon : BoardJudgmentObj
    {
        private PlayerController player;

        private Vector3 destPos;

        private GameObject curUser;

        private void Start()
        {
            player = FindObjectOfType<PlayerController>();
            ownCollider = GetComponent<BoxCollider>();
            SetTriggerState(true);
        }

        public override void OnEnter()
        {
            RaycastHit hit;
            int layerMask = (-1) - (1 << LayerMask.NameToLayer("Ignore Raycast"));

            if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 100, layerMask))
            {
                player.PlayerInteract.SetCanonState(true);

                destPos.x = hit.transform.position.x;
                destPos.y = hit.transform.position.y - 0.5f;
                destPos.z = hit.transform.position.z - 1f;

                player.PlayerInteract.FiredToPosByCanon(gameObject.transform, destPos);
            }

            Debug.Log(destPos);
            Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * hit.distance, Color.red, 3);
        }

        public override void OnStay()
        {

        }

        public override void OnExit()
        {

        }

        public override void OnMouthful()
        {
            if (!IsMouthful)
                return;

            base.OnMouthful();

            gameObject.SetActive(false);
        }

        public override void OnSpit(Vector3 targetPos)
        {
            if (!IsMouthful)
                return;

            base.OnSpit(targetPos);

            gameObject.transform.position = targetPos;
            gameObject.SetActive(true);
        }
    }
}
