using UnityEngine;

namespace Hun.Obstacle
{
    public class Trampiline : BoardJudgmentObj
    {
        private Hun.Player.Player player;

        [SerializeField] private Transform[] poses;
        [SerializeField] private Transform boardPos;

        private void Start()
        {
            player = FindObjectOfType<Hun.Player.Player>();
            ownCollider = GetComponent<BoxCollider>();
            SetTriggerState(true);
        }

        public override void OnEnter()
        {
            player.SetTrampilineState(true);

            player.JumpToPosByTrampiline(poses);
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
