using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Item
{
    public class PieceStar : MonoBehaviour, IItem
    {
        public void OnEnter()
        {

        }

        public void OnExit()
        {

        }

        public void UseItem()
        {
            Destroy(gameObject);
        }
    }
}