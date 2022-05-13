using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Obstacle
{
    public class Ladder : MonoBehaviour, IObstacle
    {
        private Hun.Player.Player player;

        private void Start()
        {
            player = FindObjectOfType<Hun.Player.Player>();
        }

        public void OnEnter()
        {
            player.SetLadderState(true);
        }

        public void OnInteract()
        {

        }

        public void OnExit()
        {
            player.SetLadderState(false);
        }
    }
}