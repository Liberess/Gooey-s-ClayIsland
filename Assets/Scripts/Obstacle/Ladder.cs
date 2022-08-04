using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Obstacle
{
    public class Ladder : MonoBehaviour, IObstacle
    {
        private Hun.Player.PlayerController player;

        private void Start()
        {
            player = FindObjectOfType<Hun.Player.PlayerController>();
        }

        public void OnEnter()
        {
            player.PlayerInteract.SetLadderState(true);
        }

        public void OnInteract()
        {

        }

        public void OnExit()
        {
            player.PlayerInteract.SetLadderState(false);
        }
    }
}