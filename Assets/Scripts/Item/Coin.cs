using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Item
{
    public class Coin : MonoBehaviour, IItem
    {
        public void UseItem()
        {
            Manager.GameManager.Instance.GetCoin(1);
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