using UnityEngine;
using Hun.Player;

namespace Hun.Obstacle
{
    public class Canon : ClayBlock
    {
        private PlayerController player;

        private Vector3 destPos;

        private GameObject curUser; 

        private void Start()
        {
            player = FindObjectOfType<PlayerController>();
        }

        public override void OnEnter()
        {
            RaycastHit hit;
            int layerMask = (-1) - (1 << LayerMask.NameToLayer("Ignore Raycast"));

            if (Physics.Raycast(transform.position, transform.forward, out hit, 100, layerMask))
            {
                if(hit.distance < 1.0f)
                {
                    return;
                }

                player.PlayerInteract.SetCanonState(true);

                destPos.x = hit.transform.position.x;
                destPos.y = hit.transform.position.y - 0.5f;
                destPos.z = hit.transform.position.z - 1f;

                player.PlayerInteract.FiredToPosByCanon(transform, destPos);
            }

            Debug.Log(destPos);
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red, 3);
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

            // player오브젝트가 targetPos를 바라보는 방향으로 회전
            Vector3 dir = targetPos - player.gameObject.transform.position;

            dir.x = 0;
            dir.z = 0;

            if (45 < dir.y || dir.y <= 135)
                dir.y = 90;
            else if (135 < dir.y || dir.y <= 225)
                dir.y = 180;
            else if (225 < dir.y || dir.y <= 315)
                dir.y = -90;
            else if (315 < dir.y || dir.y <= 405)
                dir.y = 0;

            gameObject.transform.position = targetPos;
            transform.rotation = Quaternion.Euler(dir);

            gameObject.SetActive(true);
        }
    }
}
