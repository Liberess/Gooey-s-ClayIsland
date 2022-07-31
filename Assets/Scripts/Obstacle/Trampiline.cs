using UnityEngine;

namespace Hun.Obstacle
{
    public class Trampiline : BoardJudgmentObj, IObstacle
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

        public void OnEnter()
        {
            player.SetTrampilineState(true);

            OnInteract();
        }

        public void OnExit()
        {

        }

        public void OnInteract()
        {
            player.JumpToPosByTrampiline(poses);
        }
    }
}
