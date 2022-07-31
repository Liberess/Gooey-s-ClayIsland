using UnityEngine;

namespace Hun.Obstacle
{
    public class Canon : BoardJudgmentObj, IObstacle
    {
        private Hun.Player.Player player;

        private Vector3 destPos;

        private GameObject curUser;

        private void Start()
        {
            player = FindObjectOfType<Hun.Player.Player>();
            ownCollider = GetComponent<BoxCollider>();
            SetTriggerState(true);
        }

        public void OnEnter()
        {
            RaycastHit hit;
            int layerMask = (-1) - (1 << LayerMask.NameToLayer("Ignore Raycast"));

            if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 100, layerMask))
            {
                player.SetCanonState(true);

                destPos.x = hit.transform.position.x;
                destPos.y = hit.transform.position.y - 0.5f;
                destPos.z = hit.transform.position.z - 1f;

                OnInteract();
            }

            Debug.Log(destPos);
            Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * hit.distance, Color.red, 3);
        }

        public void OnExit()
        {

        }

        public void OnInteract()
        {
            player.FiredToPosByCanon(gameObject.transform, destPos);
        }
    }
}
