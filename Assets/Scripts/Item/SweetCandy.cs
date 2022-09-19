using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Item
{
    public class SweetCandy : MonoBehaviour, IItem
    {
        public void UseItem()
        {
            World.Stage.Instance.GetCandy(1);
        }

        public void OnEnter()
        {
            UseItem();
            Destroy(gameObject);
        }

        public void OnExit()
        {

        }
    }
}
