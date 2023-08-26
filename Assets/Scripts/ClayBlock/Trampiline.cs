using UnityEngine;

namespace Hun.Obstacle
{
    public class Trampiline : ClayBlock
    {
        private Hun.Player.PlayerController player;

        [SerializeField] private Animator anim;

        [SerializeField, Range(0f, 10f)] private float force = 5f;

        [SerializeField] private Transform[] poses;
        [SerializeField] private Transform boardCheckPos;

        private void Start()
        {
            anim = GetComponentInChildren<Animator>();
            player = FindObjectOfType<Hun.Player.PlayerController>();

            if(boardCheckPos == null)
                boardCheckPos = transform.GetChild(1).gameObject.transform;
        }

        public override void OnEnter()
        {
            int layerMask = (-1) - (1 << LayerMask.NameToLayer("Player"));
            bool isSuccese = Physics.Raycast(transform.position, boardCheckPos.position - transform.position, 1, layerMask);

            anim.SetTrigger("isUsed");
            player.PlayerInteract.SetTrampilineState(true);
            player.PlayerMovement.SetOverIceState(false);
            player.PlayerInteract.JumpToPosByTrampiline(force, poses, !isSuccese);

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
            dir = dir.normalized;

            if (0.5 < dir.x && dir.x <= 1)
            {
                dir = new Vector3(1, 0, 0);
            }
            else if (0 < dir.x && dir.x <= 0.5)
            {
                if (0 < dir.z)
                    dir = new Vector3(0, 0, 1);
                else
                    dir = new Vector3(0, 0, -1);
            }
            else if (-0.5 < dir.x && dir.x <= 0)
            {
                if (0 < dir.z)
                    dir = new Vector3(0, 0, 1);
                else
                    dir = new Vector3(0, 0, -1);
            }
            else if (-1 <= dir.x && dir.x <= -0.5)
            {
                dir = new Vector3(-1, 0, 0);
            }

            gameObject.transform.position = targetPos;
            transform.rotation = Quaternion.LookRotation(dir);

            gameObject.SetActive(true);
        }
    }
}
