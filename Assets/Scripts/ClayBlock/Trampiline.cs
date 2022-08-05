using UnityEngine;

namespace Hun.Obstacle
{
    public class Trampiline : ClayBlock
    {
        private Hun.Player.PlayerController player;

        [SerializeField, Range(0f, 10f)] private float force = 5f;

        [SerializeField] private Transform[] poses;
        [SerializeField] private Transform boardCheckPos;

        private void Start()
        {
            player = FindObjectOfType<Hun.Player.PlayerController>();

            if(boardCheckPos == null)
                boardCheckPos = transform.GetChild(1).gameObject.transform;
        }

        public override void OnEnter()
        {
            if (!Physics.Raycast(transform.position, boardCheckPos.position - transform.position, 1))
            {
                player.PlayerInteract.SetTrampilineState(true);
                player.PlayerInteract.JumpToPosByTrampiline(force, poses);
            }

            Debug.DrawRay(transform.position, boardCheckPos.position - transform.position, Color.red, 3);
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
