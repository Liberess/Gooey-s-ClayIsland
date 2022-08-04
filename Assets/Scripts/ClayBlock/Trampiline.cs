using UnityEngine;

namespace Hun.Obstacle
{
    public class Trampiline : BoardJudgmentObj
    {
        private Hun.Player.PlayerController player;

        [SerializeField, Range(0f, 10f)] private float force = 5f;

        [SerializeField] private Transform[] poses;
        [SerializeField] private Transform boardPos;

        private void Start()
        {
            player = FindObjectOfType<Hun.Player.PlayerController>();
            ownCollider = GetComponent<BoxCollider>();
            SetTriggerState(true);
        }

        public override void OnEnter()
        {
            player.PlayerInteract.SetTrampilineState(true);
            player.PlayerInteract.JumpToPosByTrampiline(force, poses);
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
