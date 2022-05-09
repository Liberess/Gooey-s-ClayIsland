using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Obstacle
{
    public class FootSwitch : MonoBehaviour, IObstacle
    {
        private enum FootSwitchType
        {
            Permanent,
            Press
        }

        [SerializeField] private FootSwitchType switchType;
        [SerializeField] private GameObject linkObstacle;

        public void OnEnter()
        {
            if (linkObstacle == null)
                return;

            linkObstacle.SetActive(false);
        }

        public void OnInteract()
        {
            if (linkObstacle == null)
                return;
        }

        public void OnExit()
        {
            if (linkObstacle == null)
                return;

            if(switchType == FootSwitchType.Press)
                linkObstacle.SetActive(true);
        }
    }
}